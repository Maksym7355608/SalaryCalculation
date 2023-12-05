import React, {useState} from "react";
import {OrganizationUnitDto} from "../../models/DTO";
import {RestUnitOfWork} from "../../store/rest/RestUnitOfWork";
import {handleError, user} from "../../store/actions";
import {CustomModalDialog, EModalType} from "../helpers/CustomModalDialog";
import {Button, Form} from "react-bootstrap";
import {CustomDataTable} from "../helpers/CustomDataTable";
import SelectList from "../helpers/SelectList";
import {useForm} from "react-hook-form";

const UnitSettings: React.FC<{units: OrganizationUnitDto[]}> = ({units}) => {
    const {register} = useForm<any>();
    const [organizationUnits, setUnits] = useState<OrganizationUnitDto[]>(units);
    const [showCreateModal, setShowCreateModal] = useState(false);
    const [showEditModal, setShowEditModal] = useState(false);
    const [showDeleteModal, setShowDeleteModal] = useState(false);
    const [selected, setSelected] = useState<OrganizationUnitDto | undefined>(undefined)

    const restClient = new RestUnitOfWork();

    const handleChangeState = (show: boolean, type: EModalType, id?: number) => {
        if(id)
            setSelected(organizationUnits.find(u => u.id == id))
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

        return organizationUnits.map(x => {
            return {
                name: x.name,
                parent: x.organizationUnitId ? organizationUnits.find(u => u.id == x.organizationUnitId)?.name : undefined,
                actions: renderActionRow(x.id)
            }
        });
    }

    const renderModals = () => {
        const shortUnits = organizationUnits.map(o => {return {id: o.id, name: o.name}}) ?? [];
        const createBody = [
            {id: 'unitName', label: "Назва", control: <Form.Control {...register('name')} type="text" placeholder="Введіть назву посади"/>},
            {id: 'units', label: "Підрозділ", control: <SelectList {...register('parent')} selectName={"units"} useEmpty={true} emptyName='Оберіть батьківський підрозділ' items={shortUnits}/>},
        ];
        const editBody = [
            {id: 'edit-unit-id', control: <Form.Control {...register('id')} type='number' value={selected?.id} hidden={true} id='edit-unit-id'/>},
            {id: 'edit-unitName', label: "Назва", control: <Form.Control {...register('name')} type="text" placeholder="Введіть назву посади" defaultValue={selected?.name}/>},
            {id: 'edit-units', label: "Підрозділ", control: <SelectList {...register('parent')} selectName={"units"} useEmpty={true} emptyName='Оберіть батьківський підрозділ' items={shortUnits} value={selected?.organizationUnitId}/>},
        ];
        const deleteBody = [
            {id: 'delete-unit-id', control: <Form.Control {...register('id')} id={'delete-unit-id'} type='number' value={selected?.id} hidden={true}/>}
        ];

        const handleCreate = (data: any) => {
            if(!data.name) {
                console.error('name must be not empty');
                handleError('name', `Назва не повинна бути пуста`);
                return;
            }
            if(organizationUnits.findIndex(u => u.name == data.name) !== -1){
                console.error('name must be not empty');
                handleError('name', `Назва підрозділу не має співпадати з існуючою`);
                return;
            }
            const newUnit : OrganizationUnitDto = {
                name: data.name,
                organizationUnitId : data.parent != -1 ? data.parent : null,
                organizationId : user.organization
            } as OrganizationUnitDto;
            setUnits([...organizationUnits, newUnit]);
            restClient.organization.createOrganizationUnitAsync(newUnit)
                .then(() => {console.log('org unit successfully added')});
        }

        const handleEdit = (data: any) => {
            if(!data.name) {
                console.error('name must be not empty');
                handleError('unitName', `Назва не повинна бути пуста`);
                return;
            }
            if(organizationUnits.findIndex(u => u.name == data.name && u.id !== data.id) !== -1){
                console.error('name must be not empty');
                handleError('unitName', `Назва підрозділу не має співпадати з існуючою`);
                return;
            }
            const updated = {
                id: data.id,
                organizationId: user.organization,
                organizationUnitId: data.parent != -1 ? data.parent : null,
                name: data.name
            }

            let currentUnits = organizationUnits;

            const indexToUpdate = currentUnits.findIndex(unit => unit.id === updated.id);
            if(indexToUpdate == -1) {
                console.error('error searching index');
                handleError('edit-unit-modal', 'Помилка пошуку підрозділу');
                return;
            }

            currentUnits = currentUnits.splice(indexToUpdate, 1, updated);

            setUnits(currentUnits);
            setShowEditModal(false);

            restClient.organization.updateOrganizationUnitAsync(updated)
                .then(() => {
                    console.log('org unit successfully added')
                });
        }

        const handleDelete = (data : any) => {
            restClient.organization.deleteOrganizationUnitAsync(user.organization, data.id)
                .then(() => {
                    console.log('unit successfully deleted')
                });
        }

        return (
            <>
                <CustomModalDialog id='create-unit-modal'
                                   headerText='Створення підрозділу'
                                   body={createBody}
                                   handleActionBtn={(data: any) => handleCreate(data)}
                                   show={showCreateModal} handleChangeShow={(show: boolean) => handleChangeState(show, EModalType.create)}/>
                <CustomModalDialog id='edit-unit-modal'
                                   headerText='Редагування підрозділу'
                                   body={editBody}
                                   handleActionBtn={(data: any) => handleEdit(data)}
                                   footer={{actionBtnStyle: 'warning', actionBtnText: 'Редагувати'}}
                                   show={showEditModal} handleChangeShow={(show: boolean) => handleChangeState(show, EModalType.create)}/>
                <CustomModalDialog id='delete-unit-modal'
                                   headerText='Видалення підрозділу'
                                   body={deleteBody}
                                   handleActionBtn={(data: any) => handleDelete(data)}
                                   footer={{actionBtnStyle: 'danger', actionBtnText: 'Видалити'}}
                                   show={showDeleteModal} handleChangeShow={(show: boolean) => handleChangeState(show, EModalType.delete)}/>
            </>
        );
    }

    const paginatorRight = (
        <Button variant="secondary" type="button" size="sm" onClick={() =>
            handleChangeState(true, EModalType.create)}><i className="material-icons small">add</i> Додати</Button>
    )

    const cols = [
        {field: 'name', text: 'Назва', sortable: true,},
        {field: 'parent', text: 'Батьківський підрозділ', sortable: true},
        {field: 'actions', text: '', sortable: false},
    ]

    return (
        <>
            <div className="ibox mt-3">
                <CustomDataTable columns={cols} rows={getRows()}
                                 header={{centerHead: <>Підрозділи</>}} config={{paginatorRight: paginatorRight}}/>
            </div>
            {renderModals()}
        </>

    );
}

export default UnitSettings;