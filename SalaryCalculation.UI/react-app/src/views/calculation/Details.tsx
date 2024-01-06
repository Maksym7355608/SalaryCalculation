import {useParams} from "react-router-dom";
import React, {useEffect, useState} from "react";
import {Operation, PaymentCard} from "../../models/calculation";
import RestUnitOfWork from "../../store/rest/RestUnitOfWork";
import {Card, Col, Container, Row} from "react-bootstrap";
import {PeriodCalendar} from "../../models/schedule";
import {toShortDateString} from "../../store/actions";

export default function CalculationDetail() {
    const { id } = useParams();
    const restClient = new RestUnitOfWork();

    const [card, setCard] = useState<PaymentCard>({} as PaymentCard);
    const [calendar, setCalendar] = useState<PeriodCalendar>({} as PeriodCalendar);
    const [operations, setOperations] = useState<Operation[]>([]);

    useEffect(() => {
        const fetch = async () => {
            const pc = await restClient.calculation.getPaymentCardAsync(parseInt(id as string));
            const c = await restClient.schedule.getCalendarAsync(pc.employee.id, pc.calculationPeriod);
            const op = await restClient.calculation.getOperationsAsync(pc.employee.id, pc.calculationPeriod);
            setCard(pc);
            setCalendar(c as PeriodCalendar);
            setOperations(op);
        }
        fetch();
    }, []);

    const handleZero = (n?: number) => {
        return n && n != 0 ? n : "-";
    }

    const renderOperation = (op: Operation) => {
        return (
            <>
                <Row>
                    <label className='col form-label'>Код:</label>
                    <label className='col form-label'>{op.code}</label>
                </Row>
                <Row>
                    <label className='col form-label'>Назва:</label>
                    <label className='col form-label'>{op.name}</label>
                </Row>
                <Row>
                    <label className='col form-label'>Сума:</label>
                    <label className='col form-label'>{op.amount} грн.</label>
                </Row>
                <hr/>
            </>
        );
    }

    return (
        <Container fluid>
            <div>
                <Row>
                    <Card>
                        <Card.Header className='d-flex justify-content-center'><h5>Підсумок</h5></Card.Header>
                        <Card.Body>
                            <Row>
                                <Col>
                                    <Row>
                                        <label className='col form-label'>Працівник:</label>
                                        <label className='col form-label'>{card.employee?.name}</label>
                                    </Row>
                                </Col>
                                <Col>
                                    <Row>
                                        <label className='col form-label'>Період:</label>
                                        <label className='col form-label'>{calendar.period}</label>
                                    </Row>
                                </Col>
                            </Row>
                            <Row>
                                <Col>
                                    <Row>
                                        <label className='col form-label'>Відпрацьованих днів:</label>
                                        <label className='col form-label'>{calendar.workDays}</label>
                                    </Row>
                                </Col>
                                <Col>
                                    <Row>
                                        <label className='col form-label'>Дата розрахунку:</label>
                                        <label className='col form-label'>{toShortDateString(card.calculationDate)}</label>
                                    </Row>
                                </Col>
                            </Row>
                            <Row>
                                <Col>
                                    <Row>
                                        <label className='col form-label'>Днів відпустки:</label>
                                        <label className='col form-label'>{handleZero(calendar.vacation)}</label>
                                    </Row>
                                </Col>
                                <Col>
                                    <Row>
                                        <label className='col form-label'>Дата оплати:</label>
                                        <label className='col form-label'>{toShortDateString(card.paymentDate) ?? "-"}</label>
                                    </Row>
                                </Col>
                            </Row>
                            <Row>
                                <Col>
                                    <Row>
                                        <label className='col form-label'>Днів лікарняних:</label>
                                        <label className='col form-label'>{handleZero(calendar.sick)}</label>
                                    </Row>
                                </Col>
                                <Col>
                                    <Row>
                                        <label className='col form-label'>Нарахування:</label>
                                        <label className='col form-label'>{card.accrualAmount} грн.</label>
                                    </Row>
                                </Col>
                            </Row>
                            <Row>
                                <Col>
                                    <Row>
                                        <label className='col form-label'>Відпрацьованих годин:</label>
                                        <label className='col form-label'>{calendar.hours} год</label>
                                    </Row>
                                </Col>
                                <Col>
                                    <Row>
                                        <label className='col form-label'>Утримання:</label>
                                        <label className='col form-label'>{card.maintenanceAmount} грн.</label>
                                    </Row>
                                </Col>
                            </Row>
                            <Row>
                                <Col>
                                    <Row>
                                        <label className='col form-label'>Денних:</label>
                                        <label className='col form-label'>{calendar.dayHours} год</label>
                                    </Row>
                                </Col>
                                <Col>
                                    <Row>
                                        <label className='col form-label'>Нетто:</label>
                                        <label className='col form-label'>{card.payedAmount} грн.</label>
                                    </Row>
                                </Col>
                            </Row>
                            <Row hidden={calendar.eveningHours == 0}>
                                <Col>
                                    <Row>
                                        <label className='col form-label'>Вечірніх:</label>
                                        <label className='col form-label'>{calendar.eveningHours} год</label>
                                    </Row>
                                </Col>
                                <Col></Col>
                            </Row>
                            <Row hidden={calendar.nightHours == 0}>
                                <Col>
                                    <Row>
                                        <label className='col form-label'>Нічних:</label>
                                        <label className='col form-label'>{calendar.nightHours} год</label>
                                    </Row>
                                </Col>
                                <Col></Col>
                            </Row>
                            <Row hidden={calendar.hoursH == 0}>
                                <Col>
                                    <Row>
                                        <label className='col form-label'>Святкових:</label>
                                        <label className='col form-label'>{calendar.hoursH} год</label>
                                    </Row>
                                </Col>
                                <Col></Col>
                            </Row>
                            <Row hidden={calendar.dayHoursH == 0}>
                                <Col>
                                    <Row>
                                        <label className='col form-label'>Святкових денних:</label>
                                        <label className='col form-label'>{calendar.dayHoursH} год</label>
                                    </Row>
                                </Col>
                                <Col></Col>
                            </Row>
                            <Row hidden={calendar.eveningHoursH == 0}>
                                <Col>
                                    <Row>
                                        <label className='col form-label'>Святкових вечірніх:</label>
                                        <label className='col form-label'>{calendar.eveningHoursH} год</label>
                                    </Row>
                                </Col>
                                <Col></Col>
                            </Row>
                            <Row hidden={calendar.nightHoursH == 0}>
                                <Col>
                                    <Row>
                                        <label className='col form-label'>Святкових нічних:</label>
                                        <label className='col form-label'>{calendar.nightHoursH} год</label>
                                    </Row>
                                </Col>
                                <Col></Col>
                            </Row>
                        </Card.Body>
                    </Card>
                </Row>
                <Row>
                    <Col>
                        <Card>
                            <Card.Header className='d-flex justify-content-center'><h6>Нарахування</h6></Card.Header>
                            <Card.Body>
                                {operations.filter(op => op.sign == 1).map(op => {return renderOperation(op)})}
                            </Card.Body>
                        </Card>
                    </Col>
                    <Col>
                        <Card>
                            <Card.Header className='d-flex justify-content-center'><h6>Утримання</h6></Card.Header>
                            <Card.Body>
                                {operations.filter(op => op.sign == -1).map(op => {return renderOperation(op)})}
                            </Card.Body>
                        </Card>
                    </Col>
                </Row>
            </div>
        </Container>
    );
}