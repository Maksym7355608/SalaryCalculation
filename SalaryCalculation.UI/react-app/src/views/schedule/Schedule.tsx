import {useParams, useSearchParams} from "react-router-dom";
import RestUnitOfWork from "../../store/rest/RestUnitOfWork";
import {useEffect, useState} from "react";
import {IdNamePair} from "../../models/BaseModels";
import {Container, Table} from "react-bootstrap";
import {getDaysByMonth, monthDict} from "../../store/actions";
import {EmpDay} from "../../models/schedule";

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

    useEffect(() => {
        restClient.organization.getEmployeeAsync(empId).then(res => {
            setEmployee({id: res.id, name: res.shortName})
        })
        restClient.schedule.getScheduleByEmployeeAsync(empId, periodId).then(res => {
            setSchedule(res);
        });
    }, []);

    const month = periodId%100;
    const year = periodId/100;
    const days = getDaysByMonth(month, year % 4 == 0);
    return (
        <Container fluid>
            <h4 className='d-flex justify-content-center'>{isEditMode ? `Редагування графіку за ${monthDict[month-1].toLowerCase()} працівника ${employee?.name}`
                : `Перегляд графіку за ${periodId} працівника ${employee?.name}`}</h4>
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
                    </tr>
                    <tr>
                        <td>
                            Загалом
                        </td>
                        {
                            days.map(day => {
                                const existDay = schedule?.find(s => parseInt(s.date.split('.')[0]) === parseInt(day));
                                return (
                                    <td><input type='text' className='form-control-plaintext' defaultValue={existDay?.summary} readOnly={!isEditMode}/></td>
                                )
                            })
                        }
                    </tr>
                    </tbody>
                </Table>

                <button type='submit' className='btn btn-primary' hidden={!isEditMode}>Зберегти зміни</button>
            </form>
        </Container>
    );
}