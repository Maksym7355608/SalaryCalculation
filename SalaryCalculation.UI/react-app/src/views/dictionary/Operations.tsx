import React, {useState} from "react";
import {Button, Col, Container, Form, Row} from "react-bootstrap";
import {SubmitHandler, useForm} from "react-hook-form";
import RestUnitOfWork from "../../store/rest/RestUnitOfWork";
import {Icon} from "../../componets/helpers/Icon";
import {OperationData} from "../../models/dictionaries";
import CustomDataTable from "../../componets/helpers/CustomDataTable";
import {user} from "../../store/actions";

export default function OperationsData() {
    const {register, handleSubmit} = useForm();
    const submitHandler:SubmitHandler<any> = (data) => handleSearch(data);
    const [operations, setOperations] = useState<OperationData[]>([]);

    const restClient = new RestUnitOfWork();

    const handleSearch = (data: any) => {
        data.codes = data.codes ? data.codes.trim().split(',').map((c: string) => parseInt(c)) : null;
        data['organizationId'] = user().organization;
        restClient.dictionary.searchOperationDataAsync(data).then(r => {
            setOperations(r);
        });
    }

    const columnDefs = [
        {field: 'code', text: 'Код'},
        {field: 'name', text: 'Назва'},
        {field: 'sign', text: 'Вид'},
        {field: 'description', text: 'Опис'},
        {field: 'actions', text: ''}
    ]

    const createTableRows = () => {
        const rows = operations.map(op => {
            let row = {
                code: op.code,
                name: op.name,
                sign: op.sign > 0 ? 'Нарахування' : 'Утримання',
                actions: (<>
                    <Button type='button' variant='light' size='sm'><Icon name='edit' small/></Button>
                    <Button type='button' variant='light' size='sm'><Icon name='close' small/></Button>
                </>)
            };
            return row;
        })
        return rows;
    }

    return (
        <Container fluid>
            <Form onSubmit={handleSubmit(submitHandler)} className='form-search pt-1 pb-2'>
                <Row className='w-100 ms-1'>
                    <Col>
                        <Form.Group>
                            <Form.Label>Коди</Form.Label>
                            <Form.Control {...register('codes')} type='text'/>
                        </Form.Group>
                    </Col>
                    <Col>
                        <Form.Group>
                            <Form.Label>Назва</Form.Label>
                            <Form.Control {...register('name')} type='text'/>
                        </Form.Group>
                    </Col>
                </Row>
                <div className='card-group mt-3 ms-3'>
                    <Button type='submit' variant='primary' size='sm' className='me-1'><Icon name='search' small/> Пошук</Button>
                    <Button type='reset' variant='secondary' size='sm' className='me-1'>Очистити</Button>
                </div>
            </Form>

            <div className="mt-3 mb-3">
                <CustomDataTable columns={columnDefs} rows={createTableRows()}/>
            </div>
        </Container>
    );
}