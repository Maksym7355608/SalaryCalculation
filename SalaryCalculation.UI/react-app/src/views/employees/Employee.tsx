import {useParams} from "react-router-dom";
import RestUnitOfWork from "../../store/rest/RestUnitOfWork";
import {SubmitHandler, useForm} from "react-hook-form";
import React, {useEffect, useState} from "react";
import {EmployeeModel} from "../../models/employees";
import {Button, Container, Form} from "react-bootstrap";
import {enumToIdNamePair, user} from "../../store/actions";
import SelectList from "../../componets/helpers/SelectList";
import {IdNamePair} from "../../models/BaseModels";
import {PositionDto} from "../../models/DTO";
import {EBenefit, EMarriedStatus, ESex} from "../../models/Enums";
import {
    mapEmployeeModelToCreateCommand,
    mapEmployeeModelToUpdateCommand,
    mapPositionsToIdNamePair
} from "../../store/employees";

export default function Employee() {
    const {id} = useParams();
    const restClient = new RestUnitOfWork();

    const [employee, setEmployee] = useState<EmployeeModel | undefined>(undefined);
    const [units, setUnits] = useState<IdNamePair[]>();
    const [selectedUnit, setUnit] = useState<number | undefined>();
    const [positions, setPositions] = useState<PositionDto[]>();
    const [regimes, setRegimes] = useState<IdNamePair[]>([]);
    const [isEditMode, setIsEditMode] = useState(false);
    const sexList = enumToIdNamePair(ESex);
    const marriedStatusList = enumToIdNamePair(EMarriedStatus);
    const benefitsList = enumToIdNamePair(EBenefit);

    const {register, control, handleSubmit} = useForm<EmployeeModel>({
        defaultValues: employee
    })
    const onSubmit: SubmitHandler<EmployeeModel> = (data) => isEditMode ? handleUpdate(data) : handleCreate(data);

    useEffect(() => {
        const fetch = async () => {
            if (id !== 'create') {
                const emp = await restClient.organization.getEmployeeAsync(parseInt(id as string));
                setEmployee(emp);
                setIsEditMode(true);
            }
            const shortUnits = await restClient.organization.getOrganizationUnitsShortAsync(user().organization);
            const pos = await restClient.organization.getPositionsAsync(user().organization);
            const shortRegimes = await restClient.schedule.getRegimesShortAsync(user().organization);

            setUnits(shortUnits);
            setPositions(pos);
            setRegimes(shortRegimes);
        }

        fetch();
    }, []);

    const handleUpdate = (data: EmployeeModel) => {
        const cmd = mapEmployeeModelToUpdateCommand(data);
        restClient.organization.updateEmployeeAsync(cmd)
            .then(response => {
                if (response)
                    console.log('user was updated');
                else
                    console.error('user was not updated');
            });
    }

    const handleCreate = (data: EmployeeModel) => {
        const cmd = mapEmployeeModelToCreateCommand(data);
        restClient.organization.createEmployeeAsync(cmd)
            .then(response => {
                if (response)
                    console.log('user was created');
                else
                    console.error('user was not created');
            });
    }

    return (
        <Container fluid>
            <h2>{isEditMode ? "Редагування працівника" : "Додавання працівника"}</h2>
            <Form onSubmit={handleSubmit(onSubmit)}>
                <input type='hidden' {...register('organizationId')} value={user().organization}/>

                <Form.Group className="row input-group mt-4" controlId="rollNumber">
                    <Form.Label className="col-2">Табельний номер</Form.Label>
                    <Form.Control className="col-3" type="number"
                                  placeholder="Табельний номер" {...register('rollNumber')} />
                </Form.Group>

                <Form.Group className="row input-group mt-2" controlId="firstName">
                    <Form.Label className="col-2">Ім'я</Form.Label>
                    <Form.Control className="col-3" type="text" placeholder="Ім'я" {...register('firstName')} />
                </Form.Group>

                <Form.Group className="row input-group mt-2" controlId="middleName">
                    <Form.Label className="col-2">По-батькові</Form.Label>
                    <Form.Control className="col-3" type="text" placeholder="По-батькові" {...register('middleName')} />
                </Form.Group>

                <Form.Group className="row input-group mt-2" controlId="lastName">
                    <Form.Label className="col-2">Прізвище</Form.Label>
                    <Form.Control className="col-3" type="text" placeholder="Прізвище" {...register('lastName')} />
                </Form.Group>

                <Form.Group className="row input-group mt-2" controlId="shortName">
                    <Form.Label className="col-2">Коротке ім'я</Form.Label>
                    <Form.Control className="col-3" type="text" placeholder="Коротке ім'я" {...register('shortName')} />
                </Form.Group>

                <Form.Group className="row input-group mt-2" controlId="nameGenitive">
                    <Form.Label className="col-2">Ім'я у родовому</Form.Label>
                    <Form.Control className="col-3" type="text"
                                  placeholder="Ім'я у родовому" {...register('nameGenitive')} />
                </Form.Group>

                <Form.Group className="row input-group mt-2" controlId="organizationUnitId">
                    <input type='hidden' value={selectedUnit}/>
                    <Form.Label className="col-2">Підрозділ</Form.Label>
                    <SelectList register='organizationUnitId' id={'organizationUnitId'} items={units ?? []}/>
                </Form.Group>

                <Form.Group className="row input-group mt-2" controlId="position">
                    <Form.Label className="col-2">Посада</Form.Label>
                    <SelectList register='positionId' id={'position'}
                                items={mapPositionsToIdNamePair(positions, selectedUnit)}
                                disabled={selectedUnit === undefined}/>
                </Form.Group>

                <Form.Group className="row input-group mt-2" controlId="regimes">
                    <Form.Label className="col-2">Режим</Form.Label>
                    <SelectList register='regimeId' id={'regimes'} items={regimes}
                                disabled={selectedUnit === undefined} useEmpty={true} emptyName={'--- ---'}/>
                </Form.Group>

                <Form.Group className="row input-group mt-2" controlId="dateFrom">
                    <Form.Label className="col-2">Дата прийняття</Form.Label>
                    <Form.Control className="col-3" type="date" placeholder="Ім'я" {...register('dateFrom')} />
                </Form.Group>

                <Form.Group className="row input-group mt-2" controlId="dateTo">
                    <Form.Label className="col-2">Дата звільнення</Form.Label>
                    <Form.Control className="col-3" type="date" placeholder="Ім'я" {...register('dateTo')} />
                </Form.Group>

                <Form.Group className="row input-group mt-2" controlId="sex">
                    <Form.Label className="col-2">Стать</Form.Label>
                    <SelectList register='sex' id="sex" items={sexList}/>
                </Form.Group>

                <Form.Group className="row input-group mt-2" controlId="marriedStatus">
                    <Form.Label className="col-2">Сімейний стан</Form.Label>
                    <SelectList register='marriedStatus' id="marriedStatus" items={marriedStatusList}/>
                </Form.Group>

                <Form.Group className="row input-group mt-2" controlId="benefits">
                    <Form.Label className="col-2">Пільги</Form.Label>
                     <SelectList register='benefits' id="benefits" items={benefitsList} multiple={true}/>
                </Form.Group>

                <hr/>
                <h5>Банківський рахунок</h5>
                <Form.Group className="row input-group mt-2" controlId="bankAccount.name">
                    <Form.Label className="col-2">Назва</Form.Label>
                    <Form.Control className="col-3" type='text'
                                  placeholder='Введіть назву банку' {...register('bankAccount.name')}/>
                </Form.Group>

                <Form.Group className="row input-group mt-2" controlId="bankAccount.account">
                    <Form.Label className="col-2">Рахунок</Form.Label>
                    <Form.Control className="col-3" type='text'
                                  placeholder='Введіть номер рахунку' {...register('bankAccount.account')}/>
                </Form.Group>

                <Form.Group className="row input-group mt-2" controlId="bankAccount.iban">
                    <Form.Label className="col-2">IBAN</Form.Label>
                    <Form.Control className="col-3" type='text'
                                  placeholder='Введіть IBAN' {...register('bankAccount.iban')}/>
                </Form.Group>

                <Form.Group className="row input-group mt-2" controlId="bankAccount.mfo">
                    <Form.Label className="col-2">МФО</Form.Label>
                    <Form.Control className="col-3" type='text'
                                  placeholder='Введіть мфо' {...register('bankAccount.mfo')}/>
                </Form.Group>

                <hr/>
                <h5>Контакти</h5>
                <Form.Group className="row input-group mt-2" controlId="phone">
                    <Form.Label className="col-2">Телефон</Form.Label>
                    <Form.Control className="col-3" type="phone" placeholder='+380*********' {...register('phone')} />
                </Form.Group>
                <Form.Group className="row input-group mt-2" controlId="email">
                    <Form.Label className="col-2">Емейл</Form.Label>
                    <Form.Control className="col-3" type="email" placeholder='example@mail.com' {...register('email')} />
                </Form.Group>
                <Form.Group className="row input-group mt-2" controlId="telegram">
                    <Form.Label className="col-2">Телеграм</Form.Label>
                    <Form.Control className="col-3" type="text" placeholder='@username' {...register('telegram')} />
                </Form.Group>

                <Button type='submit' variant='primary' className='mt-3'>Зберегти</Button>
            </Form>
        </Container>
    );
}