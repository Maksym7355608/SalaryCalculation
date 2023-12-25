import React, {useEffect, useState} from "react";
import {IdNamePair} from "../../models/BaseModels";
import {SubmitHandler, useForm} from "react-hook-form";
import {Button, Col, Container, Form, Row} from "react-bootstrap";
import {Calendar} from "primereact/calendar";
import {getDaysByMonth, monthDict, toPeriodString, user} from "../../store/actions";
import { Nullable } from "primereact/ts-helpers";
import SelectList from "../../componets/helpers/SelectList";
import RestUnitOfWork from "../../store/rest/RestUnitOfWork";
import CustomDataTable from "../../componets/helpers/CustomDataTable";
import {EmployeeWithSchedule} from "../../models/employees";

interface IScheduleSearchForm {
    rollNumber?: string;
    organizationUnitIds?: number[];
    positionsIds?: number[];
    period: string;
}

export default function ScheduleSearch() {
    const restClient = new RestUnitOfWork();
    const [employees, setEmployees] = useState<EmployeeWithSchedule[]>([]);
    const [units, setOrgUnits] = useState<IdNamePair[]>([]);
    const [positions, setPositions] = useState<IdNamePair[]>([]);
    const { register, handleSubmit } = useForm<IScheduleSearchForm>();
    const [period, setPeriod] = useState<Nullable<Date>>(new Date(Date.now()));
    const [sUnits, setUnits] = useState<number[] | undefined>();
    const [sPos, setPos] = useState<number[] | undefined>();
    const submitHandler : SubmitHandler<IScheduleSearchForm> = (data) => handleSearch(data);

    useEffect(() => {
        restClient.organization.getPositionsShortAsync(user().organization).then(res => {
            setPositions(res);
        });
        restClient.organization.getOrganizationUnitsShortAsync(user().organization).then(res =>{
            setOrgUnits(res);
        });
    }, []);

    const handleSearch = (data: IScheduleSearchForm) => {
        return undefined;
    }

    const getTableColumns = () => {

        let columns = [
            { field: 'name', text: 'ПІБ', sortable: true }
        ];
        if(period){
            const month = period.getMonth()+1;
            const year = period.getFullYear();
            const createColumn = (day: string) => {
                return {
                    field: `${day}.${month}.${year}`,
                    text: `${day}`,
                    sortable: false
                };
            }

            const days = getDaysByMonth(month, year % 4 == 0);
            days.map(day => {
                columns = [...columns, createColumn(day)]
            })
        }
        return columns;
    }

    const createTableRows = () => {
        return employees.map(emp => {
            let row: any = {
                name: emp.name,
            }
            emp.schedule.forEach(s => {
                row[s.day] = s.work;
            })
            return row;
        })
    }

    return (
        <Container fluid>
            <Form onSubmit={handleSubmit(submitHandler)} className="form-search pt-1 pb-2">
                <Row className='w-100 ms-1'>
                    <Col>
                        <Form.Group>
                            <Form.Label>Табельний номер</Form.Label>
                            <Form.Control {...register('rollNumber')} type='number'/>
                        </Form.Group>
                    </Col>
                    <Col>
                        <Form.Group>
                            <input type='hidden' {...register('period')} value={toPeriodString(period as Date)}/>
                            <Form.Label>Період</Form.Label>
                            <div className='w-100 d-flex from-picker' style={{height: '37.5px'}}>
                                <Calendar className='w-100' value={period} onChange={(e) => setPeriod(e.value)} view="month"
                                          dateFormat="yy-mm"/>
                            </div>
                        </Form.Group>
                    </Col>
                </Row>
                <Row className='w-100 ms-1'>
                    <Col>
                        <Form.Group>
                            <input type="hidden" value={sUnits?.toString()} {...register('organizationUnitIds')}/>
                            <Form.Label>Підрозділи</Form.Label>
                            <div className='w-100 d-flex from-picker' style={{height: '37.5px'}}>
                                <SelectList multiple={true} id='units' setState={setUnits} items={units}/>
                            </div>
                        </Form.Group>
                    </Col>
                    <Col>
                        <Form.Group>
                            <input type="hidden" value={sPos?.toString()} {...register('positionsIds')}/>
                            <Form.Label>Посади</Form.Label>
                            <div className='w-100 d-flex from-picker' style={{height: '37.5px'}}>
                                <SelectList multiple={true} id='positions' setState={setPos} items={positions}/>
                            </div>
                        </Form.Group>
                    </Col>
                </Row>
                <div className='card-group mt-3 ms-3'>
                    <Button type='submit' variant='primary' size='sm' className='me-1'><i className='material-icons small'>search</i> Пошук</Button>
                    <Button type='reset' variant='secondary' size='sm' className='me-1'>Очистити</Button>
                    <Button type='button' variant='light' size='sm' className='me-1'><i className='material-icons small'>autorenew</i> Автозаповнення</Button>
                    <Button type='button' variant='warning' size='sm'><i className='material-icons small'>schedule</i> Режими</Button>
                </div>
            </Form>
                <div className="mt-3 mb-3">
                    <CustomDataTable columns={getTableColumns()} rows={createTableRows()} header={{
                        centerHead: <p>{period ? monthDict[period.getMonth()] : undefined}</p>
                    }}/>
                </div>

        </Container>
    );
}