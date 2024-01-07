import {Link, useParams, useSearchParams} from "react-router-dom";
import RestUnitOfWork from "../../store/rest/RestUnitOfWork";
import {useEffect, useState} from "react";
import {IdNamePair} from "../../models/BaseModels";
import {Card, Col, Container, Row, Table} from "react-bootstrap";
import {
    clearErrors,
    getDaysByMonth, handleError,
    monthDict,
    nextPeriod,
    previousPeriod,
    toShortDateString, user
} from "../../store/actions";
import {EmpDay, PeriodCalendar} from "../../models/schedule";
import { Icon } from "../../componets/helpers/Icon";
import {isNumeric} from "jquery";
import {isNumber} from "util";

interface IHourTemplate {
    date: string;
    day?: number;
    evening?: number;
    night?: number;
    summary?: string;
}

export default function Schedule() {
    const { id, period } = useParams();
    const [searchParams, setSearchParams] = useSearchParams();
    const handler = searchParams.get('handler');
    const restClient = new RestUnitOfWork();
    const isEditMode = handler?.toLowerCase() == 'edit';
    const empId = parseInt(id as string);
    const periodId = parseInt(period as string);
    const month = periodId%100;
    const year = periodId/100;
    const days = getDaysByMonth(month, year % 4 == 0);

    const [employee, setEmployee] = useState<IdNamePair>();
    const [periodCalendar, setCalendar] = useState<PeriodCalendar>();
    const [periodDays, setPeriodDays] = useState<IHourTemplate[]>([]);
    const [key, setKey] = useState('');

    useEffect(() => {
        clearErrors();
        restClient.organization.getEmployeeAsync(empId).then(res => {
            setEmployee({id: res.id, name: res.nameGenitive})
        })
        restClient.schedule.getScheduleByEmployeeAsync(empId, periodId).then(res => {
            setPeriodDays(days.map(day => {
                const existDay = res?.find(s => parseInt(s.date.split('.')[0]) === parseInt(day))
                return {
                    date: toShortDateString(new Date(year, month - 1, parseInt(day))),
                    day: existDay?.day,
                    evening: existDay?.evening,
                    night: existDay?.night,
                    summary: existDay?.summary,
                } as IHourTemplate
            }))
        });
        restClient.schedule.getCalendarAsync(empId, periodId).then(res => {
            setCalendar(res);
        })
    }, [period]);

    const handleZero = (n?: number) => {
        return n && n != 0 ? n : "-";
    }

    const handleSubmit = (event: any) => {
        event.preventDefault();
        let actualData = periodDays.filter(x => x.summary);
        if(actualData.some(x => (!isNaN(parseInt(x.summary ?? '')) && x.summary != 'V' && x.summary != 'L')
            && (x.day ==undefined || x.evening ==undefined || x.night ==undefined))){
            handleError('input', 'Введіть час у інші поля колонки')
            return;
        }
        const command = {
            organizationId: user().organization,
            employeeId: empId,
            hours: actualData
        };
        restClient.schedule.updateWorkDaysAsync(command);
    }

    const handleChangeField = (value: string, index: number, field: 'day' | 'evening' | 'night') => {
        const updatedData = periodDays.map((item, i) => {
            if (i === index) {
                const v = value ? parseFloat(value) : undefined
                let val: IHourTemplate = {
                    date: item.date,
                    [field]: v
                };
                if(field != "day" && v != undefined)
                    val.day = item?.day ?? 0;
                if(field != "evening" && v != undefined)
                    val.evening = item?.evening ?? 0;
                if(field != "night" && v != undefined)
                    val.night = item?.night ?? 0;
                if(v != undefined && val.day != undefined && val.evening != undefined && val.night != undefined)
                    val.summary = (val.day + val.evening + val.night).toString();
                return val;
            }
            return item;
        });
        setKey(`update_${index}_${field}_to_${value}`)
        setPeriodDays([...updatedData]);
    }

    const handleChangeSummary = (value: string, index: number) => {
        const parsed = parseInt(value);
        if (value && isNaN(parsed) && value !== 'V' && value !== 'L')
        {
            handleError('input', 'Некоректно введені дані, можна використовувати тільки цифри та символи V-відпустка L-лікарняний');
            return;
        }
        const updatedData = periodDays.map((item, i) => {
            if (i === index) {
                return {
                    date: item.date,
                    summary: value ? value : undefined,
                };
            }
            return item;
        });
        setKey(`update_${index}_summary_to_${value}`)
        setPeriodDays([...updatedData]);
    }

    const handleCalculate = () => {
        restClient.schedule.calculatePeriodCalendarAsync(periodId, empId).then(r => {
            if(r)
                window.location.reload();
        });
    };
    const handleCalculateSalary = () => {
        restClient.calculation.calculateEmployeeAsync({
            organizationId: user().organization,
            employeeId: empId,
            period: periodId
        });
    };
    return (
        <Container fluid>
            <h4 className='d-flex justify-content-center'>{isEditMode ? `Редагування графіку за ${monthDict[month-1].toLowerCase()} працівника ${employee?.name}`
                : `Перегляд графіку за ${monthDict[month-1].toLowerCase()} працівника ${employee?.name}`}</h4>
            <form className='mt-3' onSubmit={(event) => handleSubmit(event)} key={key}>
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
                            periodDays.map((day, index) => {
                                return (
                                    <td><input type='text' pattern="[0-9]*\.?[0-9]+" className='form-control-plaintext' value={day?.day} readOnly={!isEditMode}
                                               onChange={(event) => handleChangeField(event.target.value, index, 'day')}/></td>
                                )
                            })
                        }
                        <td>{periodDays?.reduce((a, b) => a + (b?.day ?? 0), 0)}</td>
                    </tr>
                    <tr>
                        <td>
                            Вечір
                        </td>
                        {
                            periodDays.map((day, index) => {
                                return (
                                    <td><input type='text' pattern="[0-9]*\.?[0-9]+" className='form-control-plaintext' value={day?.evening} readOnly={!isEditMode}
                                               onChange={(event) => handleChangeField(event.target.value, index, 'evening')}/></td>
                                )
                            })
                        }
                        <td>{periodDays?.reduce((a, b) => a + (b?.evening ?? 0), 0)}</td>
                    </tr>
                    <tr>
                        <td>
                            Ніч
                        </td>
                        {
                            periodDays.map((day, index) => {
                                return (
                                    <td><input type='text' pattern="[0-9]*\.?[0-9]+" className='form-control-plaintext' value={day?.night} readOnly={!isEditMode}
                                               onChange={(event) => handleChangeField(event.target.value, index, 'night')}/></td>
                                )
                            })
                        }
                        <td>{periodDays?.reduce((a, b) => a + (b?.night ?? 0), 0)}</td>
                    </tr>
                    <tr className='font-bold'>
                        <td>
                            Загалом
                        </td>
                        {
                            periodDays.map((day, index) => {
                                return (
                                    <td><input type='text' className='form-control-plaintext' value={day?.summary} readOnly={!isEditMode}
                                               onChange={(event) => handleChangeSummary(event.target.value, index)}/></td>
                                )
                            })
                        }
                        <td>{periodDays?.reduce((a, b) => a + (parseFloat(isNumeric(b?.summary) ? b?.summary ?? "0" : "0")), 0)}</td>
                    </tr>
                    </tbody>
                </Table>
                <span id='input-validation' className='text-danger'></span>
                <div className='card p-2'>
                    {periodCalendar ? (<><Card.Header><h4 className='d-flex justify-content-center'>Підсумки за місяць</h4></Card.Header>
                    <Card.Body>
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
                    </Row></Card.Body></>)
                        : <h4 className='d-flex justify-content-center'>Підсумки за місяць відсутні</h4>}
                </div>

                <div className='row'>
                    <Link to={`/schedule/${empId}/${previousPeriod(periodId)}${handler ? `?handler=${handler}` : ''}`} className='col text-secondary'
                    onClick={() => setKey(`update_to_${previousPeriod(periodId)}`)}><Icon name='arrow_back_ios'/></Link>
                    <Link to={`/schedule/${empId}/${nextPeriod(periodId)}${handler ? `?handler=${handler}` : ''}`} className='col text-secondary text-end'
                          onClick={() => setKey(`update_to_${nextPeriod(periodId)}`)}><Icon name='arrow_forward_ios'/></Link>
                </div>
                <button type='submit' className='btn btn-primary me-1' hidden={!isEditMode}>Зберегти зміни</button>
                <button type='button' className='btn btn-info' hidden={periodCalendar != undefined}
                    onClick={() => handleCalculate()}>Розрахувати період</button>
                <button type='button' className='btn btn-success' hidden={periodCalendar == undefined}
                    onClick={() => handleCalculateSalary()}><Icon name='calculate' small/> Розрахувати зарплату</button>
            </form>
        </Container>
    );
}