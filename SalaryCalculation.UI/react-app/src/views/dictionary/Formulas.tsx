import React, {useState} from "react";
import {Button, Col, Container, Form, Row} from "react-bootstrap";
import {SubmitHandler, useForm} from "react-hook-form";
import RestUnitOfWork from "../../store/rest/RestUnitOfWork";
import {Formula} from "../../models/dictionaries";
import {Icon} from "../../componets/helpers/Icon";
import CustomDataTable from "../../componets/helpers/CustomDataTable";
import {toShortDateString, user} from "../../store/actions";
import {Link, useNavigate} from "react-router-dom";

export default function Formulas() {
    const navigate = useNavigate();
    const [formulas, setFormulas] = useState<Formula[]>([]);
    const {register, handleSubmit} = useForm<any>();
    const restClient = new RestUnitOfWork();
    const submitHandler: SubmitHandler<any> = (data) => handleSearch(data);
    const handleSearch = (data: any) => {
        data.dateFrom = data.dateFrom ? data.dateFrom : undefined;
        data.dateTo = data.dateTo ? data.dateTo : undefined;
        restClient.dictionary.searchFormulasAsync(data).then(r => {
            setFormulas(r);
        });
    };

    const columnDefs = [
        {field: 'code', text: 'Код'},
        {field: 'name', text: 'Назва'},
        {field: 'expressionName', text: 'Змінна'},
        {field: 'condition', text: 'Умова'},
        {field: 'expression', text: 'Вираз'},
        {field: 'dateFrom', text: 'Дата з'},
        {field: 'dateTo', text: 'Дата по'},
        {field: 'actions', text: '', sortable: false}
    ]

    const createTableRows = () => {
        const rows = formulas.map(op => {
            let row = {
                ...op,
                dateFrom: toShortDateString(op.dateFrom),
                dateTo: toShortDateString(op.dateTo),
                actions: (<>
                    <Button type='button' variant='light' size='sm' onClick={() => navigate(`/dictionary/formula/${op.id}`)}><Icon name='edit_square' small/></Button>
                    <Button type='button' variant='light' size='sm'><Icon name='close' small/></Button>
                </>)
            };
            return row;
        })
        return rows;
    };

    return (
        <Container fluid>
            <Form onSubmit={handleSubmit(submitHandler)} className='form-search pt-1 pb-2 pe-2'>
                <input type='hidden' {...register('organizationId')} value={user().organization}/>
                <Row className='ps-3'>
                    <Col>
                        <Form.Group>
                            <Form.Label>Назва</Form.Label>
                            <Form.Control type='text' {...register('name')}/>
                        </Form.Group>
                    </Col>
                    <Col>
                        <Form.Group>
                            <Form.Label>Змінна</Form.Label>
                            <Form.Control type='text' {...register('expressionName')}/>
                        </Form.Group>
                    </Col>
                </Row>
                <Row className='ps-3'>
                    <Col>
                        <Form.Group>
                            <Form.Label>Дата з</Form.Label>
                            <Form.Control type='date' {...register('dateFrom')} defaultValue={toShortDateString(new Date(2020, 0, 1))}/>
                        </Form.Group>
                    </Col>
                    <Col>
                        <Form.Group>
                            <Form.Label>Дата по</Form.Label>
                            <Form.Control type='date' {...register('dateTo')}/>
                        </Form.Group>
                    </Col>
                </Row>
                <div className='card-group mt-3 ms-3 '>
                    <Button type='submit' variant='primary' size='sm' className='me-1'><Icon name='search' small/> Пошук</Button>
                    <Button type='reset' variant='secondary' size='sm' className='me-1'>Очистити</Button>
                    <Link to='/dictionary/formula/create' className='btn btn-sm btn-success'><Icon name='add' small/>Створити</Link>
                </div>
            </Form>

            <div className="mt-3 mb-3">
                <CustomDataTable columns={columnDefs} rows={createTableRows()}/>
            </div>
        </Container>
    );
}