import {useParams} from "react-router-dom";
import {useFieldArray, useForm} from "react-hook-form";
import {RegimeModel, WorkDetail} from "../../models/schedule";
import {Button, Col, Container, Form, Row} from "react-bootstrap";
import {useState} from "react";

export function Regime() {
    const {id} = useParams();
    const {register, control, handleSubmit} = useForm<RegimeModel>();
    const [holiday, setHoliday] = useState<boolean>(true);
    const [launch, setLaunch] = useState<boolean>(false);
    const {fields, append, remove} = useFieldArray({
        control,
        name: 'workDays', // Ім'я поля, що представляє масив об'єктів WorkDayDetailDto
    });
    const isEditMode = id?.toLowerCase() !== 'create';
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
                                <Form.Control type="number"
                                              placeholder='0-24' {...register(`workDays.${index}.startTime.hours`)}/>
                            </Col>
                            <Col>
                                <Form.Control type="number"
                                              placeholder='0-60' {...register(`workDays.${index}.startTime.minutes`)}/>
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
                                <Form.Control type="number"
                                              placeholder='0-24' {...register(`workDays.${index}.endTime.hours`)}/>
                            </Col>
                            <Col>
                                <Form.Control type="number"
                                              placeholder='0-60' {...register(`workDays.${index}.endTime.minutes`)}/>
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
                        <Form.Check {...register(`workDays.${index}.isHolidayWork`)}
                                    onChange={(event) => setHoliday(!event.target.checked)}/>
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
                <div hidden={holiday}>
                    <Row>
                        <Col md={3}>
                            <Form.Label>Початок роботи у свято</Form.Label>
                        </Col>
                        <Col md={9}>
                            <Row>
                                <Col>
                                    <Form.Control type="number"
                                                  placeholder='0-24' {...register(`workDays.${index}.startTime.hours`)}/>
                                </Col>
                                <Col>
                                    <Form.Control type="number"
                                                  placeholder='0-60' {...register(`workDays.${index}.startTime.minutes`)}/>
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
                                    <Form.Control type="number"
                                                  placeholder='0-24' {...register(`workDays.${index}.startTimeInHoliday.hours`)}/>
                                </Col>
                                <Col>
                                    <Form.Control type="number"
                                                  placeholder='0-60' {...register(`workDays.${index}.endTimeInHoliday.minutes`)}/>
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
                        <Form.Control type="number" {...register('shiftsCount')} />
                    </Col>
                </Row>
                <Row>
                    <Col md={3}>
                        <Form.Label>Дата початку цього року</Form.Label>
                    </Col>
                    <Col md={9}>
                        <Form.Control type="date" {...register('startDateInCurrentYear')}/>
                    </Col>
                </Row>
                <Row>
                    <Col md={3}>
                        <Form.Label>Дата початку попереднього року</Form.Label>
                    </Col>
                    <Col md={9}>
                        <Form.Control type="date" {...register('startDateInPreviousYear')}/>
                    </Col>
                </Row>
                <Row>
                    <Col md={3}>
                        <Form.Label>Дата початку наступного року</Form.Label>
                    </Col>
                    <Col md={9}>
                        <Form.Control type="date" {...register('startDateInNextYear')}/>
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