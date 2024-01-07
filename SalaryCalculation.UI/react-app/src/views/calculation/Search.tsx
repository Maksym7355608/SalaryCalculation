import React, {useEffect, useState} from "react";
import {SubmitHandler, useForm} from "react-hook-form";
import {PaymentCard} from "../../models/calculation";
import {PaymentCardSearchCommand} from "../../models/commands/CalculationCommand";
import {handleError, toPeriod, user} from "../../store/actions";
import RestUnitOfWork from "../../store/rest/RestUnitOfWork";
import {IdNamePair} from "../../models/BaseModels";
import {Button, Container, Form, Row} from "react-bootstrap";
import SelectList from "../../componets/helpers/SelectList";
import {Calendar} from "primereact/calendar";
import { Icon } from "../../componets/helpers/Icon";
import CustomDataTable from "../../componets/helpers/CustomDataTable";
import { Link } from "react-router-dom";
import {CustomModalDialog} from "../../componets/helpers/CustomModalDialog";
import {DeletePaymentCardModal} from "../../componets/calculation/DeletePaymentCardModal";
import MassCalculationModal from "../../componets/calculation/MassCalculationModal";

export default function CalculationSearch() {
    const [model, setModel] = useState<PaymentCard[]>([]);
    const {register, handleSubmit, formState:{errors}} = useForm<PaymentCardSearchCommand>();
    const submitHandler: SubmitHandler<PaymentCardSearchCommand> = (data) => handleSearch(data);

    const [organizationUnits, setUnits] = useState<IdNamePair[]>([]);
    const [selectedUnit, setUnit] = useState<number>();
    const [positions, setPositions] = useState<IdNamePair[]>([]);
    const [selectedPos, setPos] = useState<number>();
    const [show, setShow] = useState(false);
    const [showCalculate, setShowCalculate] = useState(false);
    const [selected, setSelected] = useState<number>();

    const restClient = new RestUnitOfWork();

    useEffect(() => {
        restClient.organization.getPositionsShortAsync(user().organization).then(res => {
            setPositions(res);
        });
        restClient.organization.getOrganizationUnitsShortAsync(user().organization).then(res =>{
            setUnits(res);
        });
    }, []);

    const handleSearch = (data: PaymentCardSearchCommand) => {
        data.organizationUnitId = selectedUnit;
        data.positionId = selectedPos;
        data.calculationPeriod = toPeriod(data.calculationPeriod);
        restClient.calculation.searchPaymentCardsAsync(data).then(res => {
            setModel(res);
        })
    }

    const createRows = () => {
        return model.map(m => {
            let row = m as any;
            row['actions'] = (
                <div>
                    <Link to={`/calculation/details/${m.id}`} className='btn btn-sm btn-light'><Icon name='info' small/></Link>
                    <Button type='button' variant='light' size='sm' onClick={() => {
                        setShow(true);
                        setSelected(m.id);
                    }}><Icon name='close' small/></Button>
                </div>
            )
            return row;
        })
    }

    const columnDefs = [
        {field: 'employee.name', text: 'ПІБ'},
        {field: 'calculationPeriod', text: 'Період'},
        {field: 'paymentDate', text: 'Дата оплати'},
        {field: 'payedAmount', text: 'На руки'},
        {field: 'accrualAmount', text: 'Нараховано'},
        {field: 'maintenanceAmount', text: 'Утримано'},
        {field: 'actions', text: '', sortable: false}
    ];
    return (
        <Container fluid>
            <Form onSubmit={handleSubmit(submitHandler)} className="form-search pt-1 pb-2 pe-2">
                <Row className='w-100 ps-4'>
                    <Form.Group className='col'>
                        <Form.Label>Табельний номер</Form.Label>
                        <Form.Control {...register('rollNumber', {pattern: /^[0-9]+$/i })}/>
                        {errors.rollNumber && <span className='text-danger'>Невірно введені дані</span>}
                    </Form.Group>
                    <Form.Group className='col'>
                        <Form.Label>Період</Form.Label>
                        <input type='month' className='form-control' {...register('calculationPeriod')}/>
                    </Form.Group>
                </Row>
                <Row className='w-100 ps-4'>
                    <Form.Group className='col'>
                        <Form.Label htmlFor="unit">Підрозділ</Form.Label>
                        <SelectList setState={(state) => setUnit(state as number)} useEmpty={true}
                                    emptyName={"Оберіть підрозділ"} id={"unit"} items={organizationUnits}/>
                    </Form.Group>
                    <Form.Group className='col'>
                        <Form.Label htmlFor="position">Посада</Form.Label>
                        <SelectList setState={(state) => setPos(state as number)} useEmpty={true}
                                    emptyName={"Оберіть посаду"} id={"position"} items={positions}/>
                    </Form.Group>
                </Row>
                <div className="row-gap-sm-0 mt-3 ps-4">
                    <button type="submit" className="col btn btn-sm btn-primary me-2" title="Пошук розрахунків">
                        <Icon name='search' small/> Пошук
                    </button>
                    <button type="reset" className="col btn btn-sm btn-warning me-2" title="Очистити фільтри">
                        Очистити
                    </button>
                    <button type="button" className="col btn btn-sm btn-success me-2" title="Розрахувати зарплату"
                    onClick={() => setShowCalculate(true)}>
                        <Icon name='calculate' small/> Розрахувати
                    </button>
                </div>
            </Form>

            <div className="mt-3 mb-3">
                <CustomDataTable columns={columnDefs} rows={createRows()}/>
            </div>

            <DeletePaymentCardModal id={selected} show={show} setShow={setShow}/>
            <MassCalculationModal show={showCalculate} setShow={setShowCalculate}/>
        </Container>
    );
}