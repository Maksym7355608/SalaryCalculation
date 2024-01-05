import {Link, useParams, useSearchParams} from "react-router-dom";
import RestUnitOfWork from "../../store/rest/RestUnitOfWork";
import {useEffect, useState} from "react";
import {IdNamePair} from "../../models/BaseModels";
import {Col, Container, Row, Table} from "react-bootstrap";
import {clearErrors, getDaysByMonth, monthDict, nextPeriod, previousPeriod} from "../../store/actions";
import {EmpDay, PeriodCalendar} from "../../models/schedule";
import { Icon } from "../../componets/helpers/Icon";

export default function Schedule() {
    const { id, period } = useParams();
    const [searchParams, setSearchParams] = useSearchParams();
    const handler = searchParams.get('handler');
    const restClient = new RestUnitOfWork();
    const isEditMode = handler?.toLowerCase() == 'edit';
    const empId = parseInt(id as string);
    const periodId = parseInt(period as string);

    const [employee, setEmployee] = useState<IdNamePair>();
    const [schedule, setSchedule] = useState<EmpDay[]>();
    const [periodCalendar, setCalendar] = useState<PeriodCalendar>()

    useEffect(() => {
        clearErrors();
        restClient.organization.getEmployeeAsync(empId).then(res => {
            setEmployee({id: res.id, name: res.nameGenitive})
        })
        restClient.schedule.getScheduleByEmployeeAsync(empId, periodId).then(res => {
            setSchedule(res);
        });
        restClient.schedule.getCalendarAsync(empId, periodId).then(res => {
            setCalendar(res);
        })
    }, [period]);

    const handleZero = (n?: number) => {
        return n && n != 0 ? n : "-";
    }

    const month = periodId%100;
    const year = periodId/100;
    const days = getDaysByMonth(month, year % 4 == 0);
    return (
        <Container fluid>
            <h4 className='d-flex justify-content-center'>{isEditMode ? `Редагування графіку за ${monthDict[month-1].toLowerCase()} працівника ${employee?.name}`
                : `Перегляд графіку за ${monthDict[month-1].toLowerCase()} працівника ${employee?.name}`}</h4>
            <form className='mt-3'>
                <Table width={100} bordered>
                    <thead>
                    <tr>
                        <th></th>
                        {
                            days.map(day => {
                                return (<th>{day}</th>)
                            })
                        }
                        <th>Σ</th>
                    </tr>
                    </thead>
                    <tbody>
                    <tr>
                        <td>
                            День
                        </td>
                        {
                            days.map(day => {
                                const existDay = schedule?.find(s => parseInt(s.date.split('.')[0]) === parseInt(day));
                                return (
                                    <td><input type='text' className='form-control-plaintext' defaultValue={existDay?.day} readOnly={!isEditMode}/></td>
                                )
                            })
                        }
                        <td>{schedule?.reduce((a, b) => a + b.day, 0)}</td>
                    </tr>
                    <tr>
                        <td>
                            Вечір
                        </td>
                        {
                            days.map(day => {
                                const existDay = schedule?.find(s => parseInt(s.date.split('.')[0]) === parseInt(day));
                                return (
                                    <td><input type='text' className='form-control-plaintext' defaultValue={existDay?.evening} readOnly={!isEditMode}/></td>
                                )
                            })
                        }
                        <td>{schedule?.reduce((a, b) => a + b.evening, 0)}</td>
                    </tr>
                    <tr>
                        <td>
                            Ніч
                        </td>
                        {
                            days.map(day => {
                                const existDay = schedule?.find(s => parseInt(s.date.split('.')[0]) === parseInt(day));
                                return (
                                    <td><input type='text' className='form-control-plaintext' defaultValue={existDay?.night} readOnly={!isEditMode}/></td>
                                )
                            })
                        }
                        <td>{schedule?.reduce((a, b) => a + b.night, 0)}</td>
                    </tr>
                    <tr className='font-bold'>
                        <td>
                            Загалом
                        </td>
                        {
                            days.map(day => {
                                const existDay = schedule?.find(s => parseInt(s.date.split('.')[0]) === parseInt(day));
                                return (
                                    <td><input type='text' className='form-control-plaintext font-bold' defaultValue={existDay?.summary} readOnly={!isEditMode}/></td>
                                )
                            })
                        }
                        <td><label className='form-control-plaintext'>{schedule?.reduce((a, b) => a + parseFloat(b.summary), 0)}</label></td>
                    </tr>
                    </tbody>
                </Table>
                <div className='card p-2'>
                    {periodCalendar ? (<><h4 className='d-flex justify-content-center'>Підсумки за місяць</h4>
                    <Row>
                        <Col sm={2}>Період:</Col>
                        <Col sm={10}>{periodCalendar?.period}</Col>
                    </Row>
                    <Row>
                        <Col sm={2}>Режим:</Col>
                        <Col sm={10}>{periodCalendar?.regime.name}</Col>
                    </Row>
                    <Row>
                        <Col sm={2}>Робочих днів:</Col>
                        <Col sm={10}>{handleZero(periodCalendar?.workDays)}</Col>
                    </Row>
                    <Row>
                        <Col sm={2}>Днів відпустки:</Col>
                        <Col sm={10}>{handleZero(periodCalendar?.vacation)}</Col>
                    </Row>
                    <Row>
                        <Col sm={2}>Днів лікарняних:</Col>
                        <Col sm={10}>{handleZero(periodCalendar?.sick)}</Col>
                    </Row>
                    <Row>
                        <Col sm={2}>Відпрацьованих годин:</Col>
                        <Col sm={10}>{handleZero(periodCalendar?.hours)}</Col>
                    </Row>
                    <Row>
                        <Col sm={2}>Денних:</Col>
                        <Col sm={10}>{handleZero(periodCalendar?.dayHours)}</Col>
                    </Row>
                    <Row>
                        <Col sm={2}>Вечірніх:</Col>
                        <Col sm={10}>{handleZero(periodCalendar?.eveningHours)}</Col>
                    </Row>
                    <Row>
                        <Col sm={2}>Нічних:</Col>
                        <Col sm={10}>{handleZero(periodCalendar?.nightHours)}</Col>
                    </Row>
                    <Row>
                        <Col sm={2}>Святкових годин:</Col>
                        <Col sm={10}>{handleZero(periodCalendar?.hoursH)}</Col>
                    </Row>
                    <Row>
                        <Col sm={2}>Святкових денних:</Col>
                        <Col sm={10}>{handleZero(periodCalendar?.dayHoursH)}</Col>
                    </Row>
                    <Row>
                        <Col sm={2}>Святкових вечірніх:</Col>
                        <Col sm={10}>{handleZero(periodCalendar?.eveningHoursH)}</Col>
                    </Row>
                    <Row>
                        <Col sm={2}>Святкових нічних:</Col>
                        <Col sm={10}>{handleZero(periodCalendar?.nightHoursH)}</Col>
                    </Row></>)
                        : <h4 className='d-flex justify-content-center'>Підсумки за місяць відсутні</h4>}
                </div>

                <div className='row'>
                    <Link to={`/schedule/${empId}/${previousPeriod(periodId)}`} className='col text-secondary'><Icon name='arrow_back_ios'/></Link>
                    <Link to={`/schedule/${empId}/${nextPeriod(periodId)}`} className='col text-secondary text-end'><Icon name='arrow_forward_ios'/></Link>
                </div>
                <button type='submit' className='btn btn-primary' hidden={!isEditMode}>Зберегти зміни</button>
            </form>
        </Container>
    );
}