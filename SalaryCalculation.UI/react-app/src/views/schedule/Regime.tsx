import {useParams} from "react-router-dom";
import {Controller, useFieldArray, useForm} from "react-hook-form";
import {RegimeModel, WorkDetail} from "../../models/schedule";
import {Button, Col, Container, Form, Row} from "react-bootstrap";
import {useEffect, useState} from "react";
import RestUnitOfWork from "../../store/rest/RestUnitOfWork";
import {getDefaultValues, toShortDateString} from "../../store/actions";


export default function Regime() {
    const {id} = useParams();
    const {register, control, handleSubmit, reset, getValues, formState: {defaultValues, errors}} = useForm<RegimeModel>();
    const [launch, setLaunch] = useState<boolean>(false);
    const {fields, append, remove} = useFieldArray({
        control,
        name: 'workDays',
    });
    const isEditMode = id?.toLowerCase() !== 'create';

    const restClient = new RestUnitOfWork();
    useEffect(() => {
        if (isEditMode)
            restClient.schedule.getRegimeAsync(parseInt(id as string))
                .then(r => {
                    reset(r);
                });
    }, []);

    const onSubmit = (data: RegimeModel) => {
        console.log(data);
    };

    const renderWorkDayDetail: React.FC<{ detail: WorkDetail, index: number }> = ({detail, index}) => {
        return (
            <div>
                <Row>
                    <Col md={3}>
                        <Form.Label>Дні циклу</Form.Label>
                    </Col>
                    <Col md={9}>
                        <Form.Control type="text"
                                      placeholder='1,2,3,4,5...' {...register(`workDays.${index}.daysOfWeek`)}/>
                    </Col>
                </Row>
                <Row>
                    <Col md={3}>
                        <Form.Label>Початок роботи</Form.Label>
                    </Col>
                    <Col md={9}>
                        <Row>
                            <Col>
                                <Form.Control placeholder='0-24' {...register(`workDays.${index}.startTime.hour`, {
                                    pattern: /^[0-9]+$/i, min: 0, max: 23
                                })}/>
                            </Col>
                            <Col>
                                <Form.Control placeholder='0-60' {...register(`workDays.${index}.startTime.minutes`, {
                                    pattern: /^[0-9]+$/i, min: 0, max: 59
                                })}/>
                            </Col>
                        </Row>
                    </Col>
                </Row>
                <Row>
                    <Col md={3}>
                        <Form.Label>Кінець роботи</Form.Label>
                    </Col>
                    <Col md={9}>
                        <Row>
                            <Col>
                                <Form.Control placeholder='0-24' {...register(`workDays.${index}.endTime.hour`, {
                                    pattern: /^[0-9]+$/i, min: 0, max: 23
                                })}/>
                            </Col>
                            <Col>
                                <Form.Control placeholder='0-60' {...register(`workDays.${index}.endTime.minutes`, {
                                    pattern: /^[0-9]+$/i, min: 0, max: 59
                                })}/>
                            </Col>
                        </Row>
                    </Col>
                </Row>
                <Row>
                    <Col md={3}>
                        <Form.Label>Чи кінець зміни наступного дня</Form.Label>
                    </Col>
                    <Col md={9}>
                        <Form.Check {...register(`workDays.${index}.isEndTimeNextDay`)}/>
                    </Col>
                </Row>
                <Row>
                    <Col md={3}>
                        <Form.Label>Святкові дні робочі</Form.Label>
                    </Col>
                    <Col md={9}>
                        <Form.Check {...register(`workDays.${index}.isHolidayWork`)}/>
                    </Col>
                </Row>
                <Row>
                    <Col md={3}>
                        <Form.Label>Передсвятковий день короткий</Form.Label>
                    </Col>
                    <Col md={9}>
                        <Form.Check {...register(`workDays.${index}.isHolidayShort`)}/>
                    </Col>
                </Row>
                <div hidden={!getValues(`workDays.${index}.isHolidayWork`)}>
                    <Row>
                        <Col md={3}>
                            <Form.Label>Початок роботи у свято</Form.Label>
                        </Col>
                        <Col md={9}>
                            <Row>
                                <Col>
                                    <Form.Control placeholder='0-24' {...register(`workDays.${index}.startTimeInHoliday.hour`, {
                                        pattern: /^[0-9]+$/i, min: 0, max: 23
                                    })}/>
                                </Col>
                                <Col>
                                    <Form.Control placeholder='0-60' {...register(`workDays.${index}.startTimeInHoliday.minutes`, {
                                        pattern: /^[0-9]+$/i, min: 0, max: 59
                                    })}/>
                                </Col>
                            </Row>
                        </Col>
                    </Row>
                    <Row>
                        <Col md={3}>
                            <Form.Label>Кінець роботи у свято</Form.Label>
                        </Col>
                        <Col md={9}>
                            <Row>
                                <Col>
                                    <Form.Control placeholder='0-24' {...register(`workDays.${index}.endTimeInHoliday.hour`, {
                                                      pattern: /^[0-9]+$/i, min: 0, max: 23
                                                  })}/>
                                </Col>
                                <Col>
                                    <Form.Control placeholder='0-60' {...register(`workDays.${index}.endTimeInHoliday.minutes`, {
                                        pattern: /^[0-9]+$/i, min: 0, max: 59
                                    })}/>
                                </Col>
                            </Row>
                        </Col>
                    </Row>
                    <Row>
                        <Col md={3}>
                            <Form.Label>Чи кінець зміни наступного дня у свято</Form.Label>
                        </Col>
                        <Col md={9}>
                            <Form.Check {...register(`workDays.${index}.isEndTimeInHolidayNextDay`)}/>
                        </Col>
                    </Row>
                </div>
                <Row>
                    <Col md={3}>
                        <Form.Label>Чи оплачуваний обід</Form.Label>
                    </Col>
                    <Col md={9}>
                        <Form.Check {...register(`workDays.${index}.isLaunchPaid`)}
                                    onChange={(event) => setLaunch(event.target.checked)}/>
                    </Col>
                </Row>
                <Row hidden={launch}>
                    <Col md={3}>
                        <Form.Label>Час на обід</Form.Label>
                    </Col>
                    <Col md={9}>
                        <Form.Control type='number' {...register(`workDays.${index}.launchTime`)}/>
                    </Col>
                </Row>
                <Button type='button' variant='warning' size='sm' onClick={() => remove(index)}>Видалити</Button>
            </div>
        )
    }

    return (
        <Container fluid>
            <h3 className='d-flex justify-content-center mb-3'>{isEditMode ? "Редагування режиму": "Створення режиму"}</h3>
            <form onSubmit={handleSubmit(onSubmit)}>
                <input type="hidden" {...register('id')} value={id}/>
                <Row>
                    <Col md={3}>
                        <Form.Label>Код</Form.Label>
                    </Col>
                    <Col md={9}>
                        <Form.Control type="text" {...register('code')} placeholder="Code"/>
                    </Col>
                </Row>
                <Row>
                    <Col md={3}>
                        <Form.Label>Назва</Form.Label>
                    </Col>
                    <Col md={9}>
                        <Form.Control type="text" {...register('name')} />
                    </Col>
                </Row>
                <Row>
                    <Col md={3}>
                        <Form.Label>Циклічний</Form.Label>
                    </Col>
                    <Col md={9}>
                        <Form.Check {...register('isCircle')}/>
                    </Col>
                </Row>
                <Row>
                    <Col md={3}>
                        <Form.Label>Вихідні дні</Form.Label>
                    </Col>
                    <Col md={9}>
                        <Form.Control type="text" {...register('restDays')} />
                    </Col>
                </Row>
                <Row>
                    <Col md={3}>
                        <Form.Label>Кількість змін</Form.Label>
                    </Col>
                    <Col md={9}>
                        <Form.Control {...register('shiftsCount', {
                            pattern: /^[0-9]+$/i, min: 1})} />
                    </Col>
                </Row>
                <Row>
                    <Col md={3}>
                        <Form.Label>Дата початку цього року</Form.Label>
                    </Col>
                    <Col md={9}>
                        <Controller
                            control={control}
                            name={"startDateInCurrentYear"}
                            render={({field: {onChange, onBlur, value, ref}}) => (
                                <Form.Control type="date" onChange={onChange} onBlur={onBlur}
                                              defaultValue={toShortDateString(value)}/>
                            )
                            }/>
                    </Col>
                </Row>
                <Row>
                    <Col md={3}>
                        <Form.Label>Дата початку попереднього року</Form.Label>
                    </Col>
                    <Col md={9}>
                        <Controller
                            control={control}
                            name={"startDateInPreviousYear"}
                            render={({field: {onChange, onBlur, value, ref}}) => (
                                <Form.Control type="date" onChange={onChange} onBlur={onBlur}
                                              defaultValue={toShortDateString(value)}/>
                            )
                            }/>
                    </Col>
                </Row>
                <Row>
                    <Col md={3}>
                        <Form.Label>Дата початку наступного року</Form.Label>
                    </Col>
                    <Col md={9}>
                        <Controller
                            control={control}
                            name={"startDateInNextYear"}
                            render={({field: {onChange, onBlur, value, ref}}) => (
                                <Form.Control type="date" onChange={onChange} onBlur={onBlur}
                                              defaultValue={toShortDateString(value)}/>
                            )
                            }/>
                    </Col>
                </Row>

                {fields.length > 0 && <h4>Шаблони робочих днів</h4>}
                {fields.map((item, index) =>
                    renderWorkDayDetail({detail: item, index: index})
                )}
                <Form.Group className='mt-1'>
                    <Button type="button" variant='secondary' size='sm' onClick={() => append({} as WorkDetail)}>Додати
                        шаблон</Button>
                </Form.Group>

                <Form.Group className='mt-3'>
                    <Button type="submit" variant='primary'>Створити</Button>
                </Form.Group>
            </form>
        </Container>
    );
}