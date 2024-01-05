import React, {useEffect, useState} from "react";
import {SubmitHandler, useForm} from "react-hook-form";
import {PaymentCard} from "../../models/calculation";
import {PaymentCardSearchCommand} from "../../models/commands/CalculationCommand";
import {toPeriod, user} from "../../store/actions";
import RestUnitOfWork from "../../store/rest/RestUnitOfWork";
import {IdNamePair} from "../../models/BaseModels";
import {Container, Form, Row} from "react-bootstrap";
import SelectList from "../../componets/helpers/SelectList";
import {Calendar} from "primereact/calendar";
import { Icon } from "../../componets/helpers/Icon";
import CustomDataTable from "../../componets/helpers/CustomDataTable";

export default function CalculationSearch() {
    const [model, setModel] = useState<PaymentCard[]>([]);
    const {register, handleSubmit} = useForm<PaymentCardSearchCommand>();
    const submitHandler: SubmitHandler<PaymentCardSearchCommand> = (data) => handleSearch(data);

    const [organizationUnits, setUnits] = useState<IdNamePair[]>([]);
    const [selectedUnit, setUnit] = useState<number>();
    const [positions, setPositions] = useState<IdNamePair[]>([]);
    const [selectedPos, setPos] = useState<number>();

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

    const columnDefs = [
        {field: 'employee.name', text: 'ПІБ'},
        {field: 'calculationPeriod', text: 'Період'},
        {field: 'paymentDate', text: 'Дата оплати'},
        {field: 'payedAmount', text: 'На руки'},
        {field: 'accrualAmount', text: 'Нараховано'},
        {field: 'maintenanceAmount', text: 'Утримано'},
    ];
    return (
        <Container fluid>
            <Form onSubmit={handleSubmit(submitHandler)} className="form-search pt-1 pb-1">
                <Row className='w-100 ps-4'>
                    <Form.Group className='col'>
                        <Form.Label>Табельний номер</Form.Label>
                        <Form.Control {...register('rollNumber', {pattern: /^[0-9]+$/i })}/>
                    </Form.Group>
                    <Form.Group className='col'>
                        <Form.Label>Період</Form.Label>
                        <div className='w-100 d-flex from-picker' style={{height: '37.5px'}}>
                            <Calendar className='w-100' {...register('calculationPeriod')} view="month"
                                      dateFormat="yy-mm"/>
                        </div>
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
                </div>
            </Form>

            <div className="mt-3 mb-3">
                <CustomDataTable columns={columnDefs} rows={model}/>
            </div>
        </Container>
    );
}