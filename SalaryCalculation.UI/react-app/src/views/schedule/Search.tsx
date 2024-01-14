import React, {useEffect, useState} from "react";
import {IdNamePair} from "../../models/BaseModels";
import {SubmitHandler, useForm} from "react-hook-form";
import {Button, Col, Container, Form, Row} from "react-bootstrap";
import {Calendar} from "primereact/calendar";
import {getDaysByMonth, monthDict, toPeriod, toPeriodString, user} from "../../store/actions";
import { Nullable } from "primereact/ts-helpers";
import SelectList from "../../componets/helpers/SelectList";
import RestUnitOfWork from "../../store/rest/RestUnitOfWork";
import CustomDataTable from "../../componets/helpers/CustomDataTable";
import {EmployeeWithSchedule} from "../../models/employees";
import {Link, useNavigate} from "react-router-dom";
import {Icon} from "../../componets/helpers/Icon";
import {AutoScheduleModal} from "../../componets/schedule/AutoScheduleModal";

interface IScheduleSearchForm {
    rollNumber?: string;
    organizationUnitIds?: number[];
    positionsIds?: number[];
    period: string;
}

export default function ScheduleSearch() {
    const restClient = new RestUnitOfWork();
    const navigate = useNavigate();
    const [employees, setEmployees] = useState<EmployeeWithSchedule[]>([]);
    const [units, setOrgUnits] = useState<IdNamePair[]>([]);
    const [positions, setPositions] = useState<IdNamePair[]>([]);
    const { register, handleSubmit, formState: { errors } } = useForm<IScheduleSearchForm>();
    const [period, setPeriod] = useState<Date>(new Date(Date.now()));
    const [sUnits, setUnits] = useState<number[] | undefined>();
    const [sPos, setPos] = useState<number[] | undefined>();
    const [show, setShow] = useState<boolean>(false);
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
        if(!period)
            return;
        let cmd = {
            organizationId: user().organization,
            period: toPeriod(data.period),
            organizationUnitIds: sUnits,
            positionIds: sPos,
        }
        restClient.schedule.getScheduleShortAsync(cmd)
            .then(res => {
                setEmployees(res);
            });
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
                    field: `${("0" + day).slice(-2)}.${("0" + month).slice(-2)}.${year}`,
                    text: `${day}`,
                    sortable: false
                };
            }

            const days = getDaysByMonth(month, year % 4 == 0);
            days.map(day => {
                columns = [...columns, createColumn(day)]
            })
        }
        columns = [...columns, {field: 'summary', text: 'Σ', sortable: true}, { field: 'actions', text: '', sortable: false }]
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
            row['summary'] = emp.schedule.reduce((a, b) => {
                const p = parseFloat(b.work);
                return a + (!isNaN(p) ? p : 0);
            }, 0);
            row['actions'] = (
                <div className='inline-flex'>
                    <Link to={`/schedule/${emp.id}/${emp.period}?handler=info`} className='btn btn-sm btn-light'><i className='material-icons small'>info</i></Link>
                    <Link to={`/schedule/${emp.id}/${emp.period}?handler=edit`} className='btn btn-sm btn-light'><i className='material-icons small'>edit</i></Link>
                </div>
            )
            return row;
        })
    }

    return (
        <Container fluid>
            <Form onSubmit={handleSubmit(submitHandler)} className="form-search pt-1 pb-2 pe-2">
                <Row className='w-100 ms-1'>
                    <Col>
                        <Form.Group>
                            <Form.Label>Табельний номер</Form.Label>
                            <Form.Control {...register('rollNumber', {pattern: /^[0-9]+$/i })} type='text'/>
                            {errors.rollNumber && <span className='text-danger'>невірно введені дані</span>}
                        </Form.Group>
                    </Col>
                    <Col>
                        <Form.Group>
                            <Form.Label>Період</Form.Label>
                            <input type='month' className='form-control' {...register('period')} defaultValue={toPeriodString(period)}
                            onChange={(event) =>{
                                const period = event.target.value.split('-').map(el => parseInt(el));
                                setPeriod(new Date(period[0], period[1]-1, 1))
                            }}/>
                        </Form.Group>
                    </Col>
                </Row>
                <Row className='w-100 ms-1'>
                    <Col>
                        <Form.Group>
                            <Form.Label>Підрозділи</Form.Label>
                            <div className='d-flex from-picker' style={{height: '37.5px'}}>
                                <SelectList multiple={true} id='units' setState={(state) => setUnits(state as number[])} items={units}/>
                            </div>
                        </Form.Group>
                    </Col>
                    <Col>
                        <Form.Group>
                            <input type="hidden" value={sPos?.toString()} {...register('positionsIds')}/>
                            <Form.Label>Посади</Form.Label>
                            <div className='w-100 d-flex from-picker' style={{height: '37.5px'}}>
                                <SelectList multiple={true} id='positions' setState={(state) => setPos(state as number[])} items={positions}/>
                            </div>
                        </Form.Group>
                    </Col>
                </Row>
                <div className='card-group mt-3 ms-3'>
                    <Button type='submit' variant='primary' size='sm' className='me-1'><Icon name='search' small/> Пошук</Button>
                    <Button type='reset' variant='secondary' size='sm' className='me-1'>Очистити</Button>
                    <Button type='button' variant='light' size='sm' className='me-1' onClick={() => setShow(!show)}><Icon name='autorenew' small/> Автоматичне табелювання</Button>
                    <Button type='button' variant='warning' size='sm' onClick={() => navigate('/dictionary/regimes')}><Icon name='schedule' small/> Режими</Button>
                </div>
            </Form>
            <div className="mt-3 mb-3">
                <CustomDataTable columns={getTableColumns()} rows={createTableRows()} header={{
                    centerHead: <p>{period ? monthDict[period.getMonth()] : undefined}</p>
                }}/>
            </div>
            <AutoScheduleModal show={show} setShow={setShow}/>
        </Container>
    );
}