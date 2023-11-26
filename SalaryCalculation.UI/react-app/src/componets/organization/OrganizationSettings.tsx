import BasePageModel from "../BasePageModel";
import {OrganizationDto, OrganizationUnitDto, PositionDto} from "../../models/DTO";
import {Button, Col, Container, Form, Row} from "react-bootstrap";
import SelectList from "../../actions/helpers/SelectList";
import "primereact/resources/themes/lara-light-indigo/theme.css";
import {CustomDataTable} from "../../actions/helpers/CustomDataTable";
import {CustomModalDialog} from "../../actions/helpers/CustomModalDialog";

interface OrganizationSettingsState {
    isLoaded : boolean;
    organization: OrganizationDto;
    organizationUnits: OrganizationUnitDto[];
    positions: PositionDto[];
    showCreateUnitModal: boolean;
    showCreatePosModal: boolean;
    showEditUnitModal: boolean;
    showEditPosModal: boolean;
    showDeleteUnitModal: boolean;
    showDeletePosModal: boolean;
    selectedUnit?: OrganizationUnitDto;
    selectedPos?: PositionDto;
}

enum EModalType {
    create,
    edit,
    delete,
    info,
}

export class OrganizationSettings extends BasePageModel<any, OrganizationSettingsState> {

    constructor() {
        super(undefined);
        this.state = {
            isLoaded: false,
            organization: {} as OrganizationDto,
            organizationUnits : [],
            positions: [],
            showCreatePosModal : false,
            showCreateUnitModal : false,
            showEditPosModal : false,
            showEditUnitModal : false,
            showDeletePosModal : false,
            showDeleteUnitModal : false,
        }
    }

    async componentDidMount() {
        document.title = "Налаштування організації";
        $('#create-unit-modal').hide();
        $('#create-position-modal').hide();
        if(this.state.isLoaded)
            return;
        let org : OrganizationDto = await this.restClient.organization.getOrganizationAsync(this.user.organization);
        let units : OrganizationUnitDto[] = await this.restClient.organization.getOrganizationUnitsAsync(this.user.organization);
        let positions : PositionDto[] = await this.restClient.organization.getPositionsAsync(this.user.organization);

        this.setState({
            isLoaded: true,
            organization: org,
            organizationUnits: units,
            positions: positions,
        });
    }

    handleChangeUnit(show: boolean, type: EModalType, id?: number) {
        this.setState((prevState) => {
            return {
            showCreateUnitModal: type == EModalType.create && show,
            showDeleteUnitModal: type == EModalType.delete && show,
            showEditUnitModal: type == EModalType.edit && show,
            selectedUnit: id ? prevState.organizationUnits.find(u => u.id == id) : undefined
        }});
    }

    handleChangePos(show: boolean, type: EModalType, id?: number) {
        this.setState((prevState) => {
            return {
            showCreatePosModal: type == EModalType.create && show,
            showDeletePosModal: type == EModalType.delete && show,
            showEditPosModal: type == EModalType.edit && show,
            selectedUnit: id ? prevState.organizationUnits.find(u => u.id == id) : undefined
        }});
    }

    getRowsWithUnits(): any[] {
        const units = this.state.organizationUnits;
        return this.state.organizationUnits.map(x => {
            return {
                name: x.name,
                parent: x.organizationUnitId ? units.find(u => u.id == x.organizationUnitId)?.name : undefined,
                actions: this.renderActionRowForUnit(x.id)
            }
        });
    }

    getRowsWithPositions(): any[] {
        const units = this.state.organizationUnits;
        return this.state.positions.map(x => {
            return {
                name: x.name,
                parent: units.find(u => u.id == x.organizationUnitId)?.name,
                actions: this.renderActionRowForPosition(x.id)
            }
        });
    }

    handleCreateUnit() {
        let units = this.state.organizationUnits;
        let name = $('#unitName').val() as string;
        if(!name) {
            console.error('name must be not empty');
            this.handleError('unitName', `Назва не повинна бути пуста`);
            return;
        }
        if(units.findIndex(u => u.name == name) !== -1){
            console.error('name must be not empty');
            this.handleError('unitName', `Назва підрозділу не має співпадати з існуючою`);
            return;
        }
        let parentId = $('#units').val() as number;
        const newUnit : OrganizationUnitDto = {
            name: name,
            organizationUnitId : parentId != -1 ? parentId : undefined,
            organizationId : this.user.organization
        } as OrganizationUnitDto;
        units.push(newUnit);
        this.setState({
            organizationUnits: units,
            showCreateUnitModal: false
        });
        this.restClient.organization.createOrganizationUnitAsync(newUnit)
            .then(() => {console.log('org unit successfully added')});
    }

    handleEditUnit() {
        let id = parseInt($('#edit-unit-id').val() as string);
        let name = $('#edit-unitName').val() as string;
        if(!name) {
            console.error('name must be not empty');
            this.handleError('unitName', `Назва не повинна бути пуста`);
            return;
        }
        if(this.state.organizationUnits.findIndex(u => u.name == name && u.id !== id) !== -1){
            console.error('name must be not empty');
            this.handleError('unitName', `Назва підрозділу не має співпадати з існуючою`);
            return;
        }
        let parentId = $('#edit-units').val() as number;

        const updated = {
            id: id,
            organizationId: this.user.organization,
            organizationUnitId: parentId,
            name: name
        }

        let currentUnits = this.state.organizationUnits;

        const indexToUpdate = currentUnits.findIndex(unit => unit.id === updated.id);
        if(indexToUpdate == -1) {
            console.error('error searching index');
            this.handleError('edit-unit-modal', 'Помилка пошуку підрозділу');
            return;
        }

        currentUnits = currentUnits.splice(indexToUpdate, 1, updated);

        this.setState({
            organizationUnits: currentUnits,
            showEditUnitModal: false
        });

        this.restClient.organization.updateOrganizationUnitAsync(updated)
            .then(() => {
                console.log('org unit successfully added')
            });
    }

    handleCreatePosition() {
        let name = $('#posName').val() as string;
        if(!name) {
            console.error('name must be not empty');
            this.handleError('posName', `Назва не повинна бути пуста`);
            return;
        }
        if(this.state.positions.findIndex(u => u.name == name) !== -1){
            console.error('name must be not equal with exists');
            this.handleError('posName', `Назва підрозділу не має співпадати з існуючою`);
            return;
        }
        let parentId = $('#posUnit').val() as number;
        let positions = this.state.positions;
        const newPos : PositionDto = {
            name: name,
            organizationUnitId : parentId,
            organizationId : this.user.organization
        } as PositionDto;
        positions.push(newPos);
        this.setState({
            positions: positions,
            showCreatePosModal: false
        });

        this.restClient.organization.createPositionAsync(newPos)
            .then(() => {console.log('position successfully added')});
    }

    handleEditPosition() {
        let id = parseInt($('#edit-pos-id').val() as string);
        let name = $('#edit-posName').val() as string;
        if(!name) {
            console.error('name must be not empty');
            this.handleError('edit-posName', `Назва не повинна бути пуста`);
            return;
        }
        if(this.state.positions.findIndex(u => u.name == name && u.id !== id) !== -1){
            console.error('name must be not equal with exists');
            this.handleError('edit-posName', `Назва підрозділу не має співпадати з існуючою`);
            return;
        }
        let parentId = parseInt($('#edit-posUnit').val() as string);
        const updated : PositionDto = {
            id: id,
            name: name,
            organizationUnitId : parentId,
            organizationId : this.user.organization
        };

        let currentPos = this.state.positions;

        const indexToUpdate = currentPos.findIndex(p => p.id === updated.id);
        if(indexToUpdate == -1) {
            console.error('error searching index');
            this.handleError('edit-position-modal', 'Помилка пошуку Посади');
            return;
        }

        currentPos = currentPos.splice(indexToUpdate, 1, updated);

        this.setState({
            positions: currentPos,
            showEditPosModal: false
        });

        this.restClient.organization.updatePositionAsync(updated)
            .then(() => {
                console.log('position successfully added')
            });
    }

    handleDeleteUnit() {
        let id = parseInt($('#delete-unit-id').val() as string);
        this.restClient.organization.deleteOrganizationUnitAsync(this.user.organization, id)
            .then(() => {
                console.log('unit successfully deleted')
            });
    }

    handleDeletePosition() {
        let id = parseInt($('#delete-pos-id').val() as string);
        let unitId = parseInt($('#delete-pos-unit-id').val() as string);
        this.restClient.organization.deletePositionAsync(this.user.organization, unitId, id)
            .then(() => {
                console.log('position successfully deleted')
            });
    }

    renderActionRowForUnit(id: number) {
        return (
            <div className="btn-group small">
                <button type="button" className="btn btn-sm btn-light" onClick={() => this.handleChangeUnit(true, EModalType.edit, id)}><i className="material-icons small">edit_square</i></button>
                <button type="button" className="btn btn-sm btn-light" onClick={() => this.handleChangeUnit(true, EModalType.delete, id)}><i className="material-icons small">close</i></button>
            </div>
        );
    }

    renderActionRowForPosition(id: number) {
        return (
            <div className="btn-group">
                <button type="button" className="btn btn-sm btn-light" onClick={() => this.handleChangePos(true, EModalType.edit, id)}><i className="material-icons small">edit_square</i></button>
                <button type="button" className="btn btn-sm btn-light" onClick={() => this.handleChangePos(true, EModalType.delete, id)}><i className="material-icons small">close</i></button>
            </div>
        );
    }

    renderModals() {
        let shortUnits = this.state.organizationUnits.map(o => {return {id: o.id, name: o.name}}) ?? [];
        let createPosBody = [
            {id: 'posName', label: "Назва", control: <Form.Control type="text" placeholder="Введіть назву посади"/>},
            {id: 'posUnit', label: "Підрозділ", control: <SelectList selectName={"posUnit"} items={shortUnits}/>},
        ];
        let createUnitBody = [
            {id: 'unitName', label: "Назва", control: <Form.Control type="text" placeholder="Введіть назву посади"/>},
            {id: 'units', label: "Підрозділ", control: <SelectList selectName={"units"} useEmpty={true} emptyName='Оберіть батьківський підрозділ' items={shortUnits}/>},
        ];
        let editUnitBody = [
            {id: 'edit-unit-id', control: <Form.Control type='number' value={this.state.selectedUnit?.id} hidden={true} id='edit-unit-id'/>},
            {id: 'edit-unitName', label: "Назва", control: <Form.Control type="text" placeholder="Введіть назву посади" defaultValue={this.state.selectedUnit?.name}/>},
            {id: 'edit-units', label: "Підрозділ", control: <SelectList selectName={"units"} useEmpty={true} emptyName='Оберіть батьківський підрозділ' items={shortUnits} value={this.state.selectedUnit?.organizationUnitId}/>},
        ];
        let editPosBody = [
            {id: 'edit-pos-id', control: <Form.Control type='number' value={this.state.selectedPos?.id} hidden={true} id='edit-pos-id'/>},
            {id: 'edit-posName', label: "Назва", control: <Form.Control type="text" placeholder="Введіть назву посади" defaultValue={this.state.selectedPos?.name}/>},
            {id: 'edit-posUnit', label: "Підрозділ", control: <SelectList selectName={"units"} items={shortUnits} value={this.state.selectedPos?.organizationUnitId}/>},
        ];
        return (
            <>
                <CustomModalDialog id='create-unit-modal'
                                   headerText='Створення підрозділу'
                                   body={createUnitBody}
                                   handleActionBtn={() => this.handleCreateUnit()}
                                   show={this.state.showCreateUnitModal} handleChangeShow={(show: boolean) => this.handleChangeUnit(show, EModalType.create)}/>

                <CustomModalDialog id='create-position-modal'
                                   headerText='Створення посади'
                                   body={createPosBody}
                                   handleActionBtn={() => this.handleCreatePosition()}
                                   show={this.state.showCreatePosModal} handleChangeShow={(show: boolean) => this.handleChangePos(show, EModalType.create)}/>

                <CustomModalDialog id='edit-unit-modal'
                                   headerText='Редагування підрозділу'
                                   body={editUnitBody}
                                   handleActionBtn={() => this.handleEditUnit()}
                                   footer={{actionBtnStyle: 'warning', actionBtnText: 'Редагувати'}}
                                   show={this.state.showEditUnitModal} handleChangeShow={(show: boolean) => this.handleChangeUnit(show, EModalType.create)}/>

                <CustomModalDialog id='edit-position-modal'
                                   headerText='Редагування посади'
                                   body={editPosBody}
                                   handleActionBtn={() => this.handleEditPosition()}
                                   footer={{actionBtnStyle: 'warning', actionBtnText: 'Редагувати'}}
                                   show={this.state.showEditPosModal} handleChangeShow={(show: boolean) => this.handleChangePos(show, EModalType.create)}/>

                <CustomModalDialog id='delete-unit-modal'
                                   headerText='Видалення підрозділу'
                                   body={[{id: 'delete-unit-id', control: <Form.Control id={'delete-unit-id'} type='number' value={this.state.selectedUnit?.id} hidden={true}/>},]}
                                   handleActionBtn={() => this.handleDeleteUnit()}
                                   footer={{actionBtnStyle: 'danger', actionBtnText: 'Видалити'}}
                                   show={this.state.showDeleteUnitModal} handleChangeShow={(show: boolean) => this.handleChangeUnit(show, EModalType.delete)}/>

                <CustomModalDialog id='delete-pos-modal'
                                   headerText='Видалення посади'
                                   body={[{id: 'delete-pos-id', control: <Form.Control id={'delete-pos-id'} type='number' value={this.state.selectedPos?.id} hidden={true}/>},
                                       {id: 'delete-pos-unit-id', control: <Form.Control id={'delete-pos-unit-id'} type='number' value={this.state.selectedPos?.organizationUnitId} hidden={true}/>}]}
                                   handleActionBtn={() => this.handleDeletePosition()}
                                   footer={{actionBtnStyle: 'danger', actionBtnText: 'Видалити'}}
                                   show={this.state.showDeletePosModal} handleChangeShow={(show: boolean) => this.handleChangePos(show, EModalType.delete)}/>
            </>
        );
    }

    render() {
        const paginatorRightUnit = (
            <Button variant="secondary" type="button" size="sm" onClick={() => this.handleChangeUnit(true, EModalType.create)}><i className="material-icons small">add</i> Додати</Button>
        )
        const paginatorRightPos = (
            <Button variant="secondary" type="button" size="sm" onClick={() => this.handleChangePos(true, EModalType.create)}><i className="material-icons small">add</i> Додати</Button>
        );
        const colsUnits = [
            {field: 'name', text: 'Назва', sortable: true,},
            {field: 'parent', text: 'Батьківський підрозділ', sortable: true},
            {field: 'actions', text: '', sortable: false},
        ]
        const colsPos = [
            {field: 'name', text: 'Назва', sortable: true},
            {field: 'parent', text: 'Підрозділ', sortable: true},
            {field: 'actions', text: '', sortable: false},
        ]
        return (
            <Container fluid>
                <Row className="d-flex justify-content-center">
                    <h4 className="text-center">Організація</h4>
                </Row>
                <Row>
                    <Col><button className="btn btn-toolbar">Налаштування прав доступу</button></Col>
                    <Col><button className="btn btn-toolbar">Редагування організації</button></Col>
                </Row>
                <Row>
                    <Col><button className="btn btn-toolbar">Перегляд організацій</button></Col>
                    <Col><button className="btn btn-toolbar">Створення організації</button></Col>
                </Row>
                <div className="ibox mt-3">
                    <CustomDataTable columns={colsUnits} rows={this.getRowsWithUnits()}
                                     header={{centerHead: <>Підрозділи</>}} config={{paginatorRight: paginatorRightUnit}}/>
                </div>
                <div className="ibox mt-3 mb-3">
                    <CustomDataTable columns={colsPos} rows={this.getRowsWithPositions()}
                                     header={{centerHead: <>Посади</>}} config={{paginatorRight: paginatorRightPos}}/>
                </div>
                {this.renderModals()}
            </Container>
        );
    }
}