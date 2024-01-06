import {useEffect, useState} from "react";
import {IdNamePair} from "../../models/BaseModels";
import {SubmitHandler, useForm} from "react-hook-form";
import RestUnitOfWork from "../../store/rest/RestUnitOfWork";
import {toShortDateString, user} from "../../store/actions";
import {Button, Card, Col, Container, Form, Row} from "react-bootstrap";
import SelectList from "../../componets/helpers/SelectList";
import {useParams} from "react-router-dom";
import {Formula} from "../../models/dictionaries";

export default function IndexFormula() {
    const {id} = useParams();
    const restClient = new RestUnitOfWork();
    const [operations, setOperations] = useState<IdNamePair[]>([]);
    useEffect(() => {
        restClient.dictionary.searchOperationDataShortAsync({organizationId: user().organization}).then(r => {
            setOperations(r);
            setCode(r[0].id);
        });
        if(isEdit)
            restClient.dictionary.searchFormulasAsync({organizationId: user().organization, id: id}).then(r => {
                setFormula(r[0]);
            })
    }, []);

    const [isEdit, setIsEdit] = useState(id != 'create');
    const [formula, setFormula] = useState<Formula>();
    const [code, setCode] = useState(0);

    const {register, handleSubmit} = useForm();
    const submitHandler: SubmitHandler<any> = (data: any) => handleCreate(data);
    const handleCreate = async (data: any) => {
        data['code'] = code;
        data.dateTo = data.dateTo ? data.dateTo : undefined;
        if(isEdit){
            restClient.dictionary.updateFormulaAsync(formula?.id as string, data).then(r => {
                if (r.isSuccess) {
                    console.log('success');
                    setIsEdit(true);
                } else
                    console.error('invalid data');
            })
        } else{
            restClient.dictionary.createFormulaAsync(data).then(r => {
                if (r.isSuccess) {
                    console.log('success');
                    setIsEdit(true);
                } else
                    console.error('invalid data');
            })
        }

    };

    return (
        <Container fluid>
            <Card>
                <Card.Header><h4>{isEdit ? "Редагування формули" : "Створення формули"}</h4></Card.Header>
                <Form onSubmit={handleSubmit(submitHandler)}>
                    <Card.Body>
                        <input type='hidden' value={user().organization} {...register('organizationId')}/>
                        <Row>
                            <Form.Label className='col-2'>Код</Form.Label>
                            <Col>
                                <SelectList id='codes' items={operations}
                                            setState={(state) => setCode(state as number)}
                                            value={formula?.code} disabled={isEdit}/>
                            </Col>
                        </Row>
                        <Row>
                            <Form.Label className='col-2'>Назва</Form.Label>
                            <Col>
                                <Form.Control type='text' {...register('name', {required: true})} defaultValue={formula?.name}/>
                            </Col>
                        </Row>
                        <Row>
                            <Form.Label className='col-2'>Умова виконання</Form.Label>
                            <Col>
                                <Form.Control {...register('condition')} defaultValue={formula?.condition}/>
                            </Col>
                        </Row>
                        <Row>
                            <Form.Label className='col-2'>Вираз</Form.Label>
                            <Col>
                                <Form.Control {...register('expression', {required: true})} defaultValue={formula?.expression}/>
                            </Col>
                        </Row>
                        <Row>
                            <Form.Label className='col-2'>Назва змінної</Form.Label>
                            <Col>
                                <Form.Control {...register('expressionName', {required: true})} defaultValue={formula?.expressionName}/>
                            </Col>
                        </Row>
                        <Row>
                            <Form.Label className='col-2'>Дата з</Form.Label>
                            <Col>
                                <Form.Control type='date' {...register('dateFrom', {required: true})}
                                              defaultValue={toShortDateString(isEdit ? formula?.dateFrom : new Date(Date.now()))}
                                              disabled={isEdit}
                                />
                            </Col>
                        </Row>
                        <Row>
                            <Form.Label className='col-2'>Дата по</Form.Label>
                            <Col>
                                <Form.Control type='date' {...register('dateTo')}
                                    defaultValue={toShortDateString(formula?.dateTo)}
                                    disabled={!!(formula?.dateTo ?? false)}/>
                            </Col>
                        </Row>
                    </Card.Body>
                    <Card.Footer>
                        <Button type='submit' variant='primary'>Зберегти</Button>
                    </Card.Footer>
                </Form>
            </Card>
        </Container>
    );
}