import React, {useEffect, useState} from "react";
import {OrganizationUnitDto, PositionDto} from "../../models/DTO";
import {Button, Form} from "react-bootstrap";
import {handleError, user} from "../../store/actions";
import RestUnitOfWork from "../../store/rest/RestUnitOfWork";
import {CustomModalDialog, EModalType} from "../helpers/CustomModalDialog";
import {useForm} from "react-hook-form";
import CustomDataTable from "../helpers/CustomDataTable";
import SelectList from "../helpers/SelectList";

const PositionSettings: React.FC<{units: OrganizationUnitDto[]}> = ({units}) => {
    const restClient = new RestUnitOfWork();
    const {register} = useForm();

    const [positions, setPositions] = useState<PositionDto[]>([]);
    const [showCreateModal, setShowCreateModal] = useState(false);
    const [showEditModal, setShowEditModal] = useState(false);
    const [showDeleteModal, setShowDeleteModal] = useState(false);
    const [selected, setSelected] = useState<OrganizationUnitDto | undefined>(undefined);

    const [sUnit, setUnit] = useState<number | undefined>();

    useEffect(() => {
        restClient.organization.getPositionsAsync(user().organization)
            .then(result => {
                setPositions(result);
            });
    }, []);

    const handleChangeState = (show: boolean, type: EModalType, id?: number) => {
        if(id)
            setSelected(positions.find(u => u.id == id))
        switch (type) {
            case EModalType.create:
                setShowCreateModal(show);
                return;
            case EModalType.edit:
                setShowEditModal(show);
                return;
            case EModalType.delete:
                setShowDeleteModal(show);
                return;
            default:
                return;
        }
    }

    const getRows = (): any[] => {
        const renderActionRow = (id: number) => {
            return (
                <div className="btn-group small">
                    <button type="button" className="btn btn-sm btn-light"
                            onClick={() => handleChangeState(true, EModalType.edit, id)}><i
                        className="material-icons small">edit_square</i></button>
                    <button type="button" className="btn btn-sm btn-light"
                            onClick={() => handleChangeState(true, EModalType.delete, id)}><i
                        className="material-icons small">close</i></button>
                </div>
            );
        }

        return positions.map(x => {
            return {
                name: x.name,
                parent: x.organizationUnitId ? positions.find(u => u.id == x.organizationUnitId)?.name : undefined,
                actions: renderActionRow(x.id)
            }
        });
    }

    const renderModals = () => {
        const shortUnits = units.map(o => {return {id: o.id, name: o.name}}) ?? [];
        const createBody = [
            {id: 'pos-name', label: "Назва", control: <Form.Control {...register('name')} type="text" placeholder="Введіть назву посади"/>},
            {id: 'pos-units', label: "Підрозділ", control: <SelectList setState={(state) => setUnit(state as number)} id={"units"} useEmpty={true} emptyName='Оберіть батьківський підрозділ' items={shortUnits}/>},
        ];
        const editBody = [
            {id: 'edit-pos-id', control: <Form.Control {...register('id')} type='number' value={selected?.id} hidden={true} id='edit-pos-id'/>},
            {id: 'edit-pos', label: "Назва", control: <Form.Control {...register('name')} type="text" placeholder="Введіть назву посади" defaultValue={selected?.name}/>},
            {id: 'edit-units', label: "Підрозділ", control: <SelectList setState={(state) => setUnit(state as number)} id={"units"} useEmpty={true} emptyName='Оберіть батьківський підрозділ' items={shortUnits} value={selected?.organizationUnitId}/>},
        ];
        const deleteBody = [
            {id: 'delete-pos-id', control: <Form.Control {...register('id')} id={'delete-pos-id'} type='number' value={selected?.id} hidden={true}/>},
            {id: 'delete-unit-id', control: <Form.Control {...register('unitId')} id={'delete-unit-id'} type='number' value={selected?.id} hidden={true}/>}
        ];

        const handleCreate = (data: any) => {
            data['parent'] = sUnit;
            if(!data.name) {
                console.error('name must be not empty');
                handleError('pos-name', `Назва не повинна бути пуста`);
                return;
            }
            if(positions.findIndex(u => u.name == data.name) !== -1){
                console.error('name must be not empty');
                handleError('pos-name', `Назва підрозділу не має співпадати з існуючою`);
                return;
            }
            const pos : PositionDto = {
                name: data.name,
                organizationUnitId : data.parent != -1 ? data.parent : null,
                organizationId : user().organization
            } as PositionDto;
            setPositions([...positions, pos]);
            restClient.organization.createPositionAsync(pos)
                .then(() => {console.log('position successfully added')});
        }

        const handleEdit = (data: any) => {
            data['parent'] = sUnit;
            if(!data.name) {
                console.error('name must be not empty');
                handleError('edit-pos', `Назва не повинна бути пуста`);
                return;
            }
            if(positions.findIndex(u => u.name == data.name && u.id !== data.id) !== -1){
                console.error('name must be not empty');
                handleError('edit-pos', `Назва підрозділу не має співпадати з існуючою`);
                return;
            }
            const updated = {
                id: data.id,
                organizationId: user().organization,
                organizationUnitId: data.parent != -1 ? data.parent : null,
                name: data.name
            } as PositionDto;

            let current = positions;

            const indexToUpdate = current.findIndex(unit => unit.id === updated.id);
            if(indexToUpdate == -1) {
                console.error('error searching index');
                handleError('edit-position-modal', 'Помилка пошуку підрозділу');
                return;
            }

            current = current.splice(indexToUpdate, 1, updated);

            setPositions(current);
            setShowEditModal(false);

            restClient.organization.updatePositionAsync(updated)
                .then(() => {
                    console.log('position successfully updated')
                });
        }

        const handleDelete = (data : any) => {
            restClient.organization.deletePositionAsync(user().organization, data.unitId, data.id)
                .then(() => {
                    console.log('position successfully deleted')
                });
        }

        return (
            <>
                <CustomModalDialog id='create-position-modal'
                                   headerText='Створення підрозділу'
                                   body={createBody}
                                   handleActionBtn={(data: any) => handleCreate(data)}
                                   show={showCreateModal} handleChangeShow={(show: boolean) => handleChangeState(show, EModalType.create)}/>
                <CustomModalDialog id='edit-position-modal'
                                   headerText='Редагування підрозділу'
                                   body={editBody}
                                   handleActionBtn={(data: any) => handleEdit(data)}
                                   footer={{actionBtnStyle: 'warning', actionBtnText: 'Редагувати'}}
                                   show={showEditModal} handleChangeShow={(show: boolean) => handleChangeState(show, EModalType.edit)}/>
                <CustomModalDialog id='delete-position-modal'
                                   headerText='Видалення підрозділу'
                                   body={deleteBody}
                                   handleActionBtn={(data: any) => handleDelete(data)}
                                   footer={{actionBtnStyle: 'danger', actionBtnText: 'Видалити'}}
                                   show={showDeleteModal} handleChangeShow={(show: boolean) => handleChangeState(show, EModalType.delete)}/>
            </>
        );
    }

    const paginatorRight = (
        <Button variant="secondary" type="button" size="sm" onClick={() => handleChangeState(true, EModalType.create)}><i className="material-icons small">add</i> Додати</Button>
    );
    const cols = [
        {field: 'name', text: 'Назва', sortable: true},
        {field: 'parent', text: 'Підрозділ', sortable: true},
        {field: 'actions', text: '', sortable: false},
    ]
    return (
        <>
            <div className="ibox mt-3">
                <CustomDataTable columns={cols} rows={getRows()}
                                 header={{centerHead: <>Посади</>}} config={{paginatorRight: paginatorRight}}/>

            </div>
            {renderModals()}
        </>

    );
}

export default PositionSettings;