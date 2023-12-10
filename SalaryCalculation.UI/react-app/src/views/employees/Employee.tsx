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
    const { id } = useParams();
    const restClient = new RestUnitOfWork();

    const [employee, setEmployee] = useState<EmployeeModel | undefined>(undefined);
    const [units, setUnits] = useState<IdNamePair[]>();
    const [selectedUnit, setUnit] = useState<number | undefined>();
    const [positions, setPositions] = useState<PositionDto[]>();
    const [regimes, setRegimes] = useState<IdNamePair[]>();
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
            if (id !== 'new') {
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
    }, [id, restClient.organization, restClient.schedule]);

    const handleUpdate = (data: EmployeeModel) => {
        const cmd = mapEmployeeModelToUpdateCommand(data);
        restClient.organization.updateEmployeeAsync(cmd)
            .then(response => {
                if(response)
                    console.log('user was updated');
                else
                    console.error('user was not updated');
            });
    }

    const handleCreate = (data: EmployeeModel) => {
        const cmd = mapEmployeeModelToCreateCommand(data);
        restClient.organization.createEmployeeAsync(cmd)
            .then(response => {
                if(response)
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

                <Form.Group controlId="rollNumber">
                    <Form.Label>Табельний номер</Form.Label>
                    <Form.Control type="number" placeholder="Табельний номер" {...register('rollNumber')} />
                </Form.Group>

                <Form.Group controlId="firstName">
                    <Form.Label>Ім'я</Form.Label>
                    <Form.Control type="text" placeholder="Ім'я" {...register('firstName')} />
                </Form.Group>

                <Form.Group controlId="middleName">
                    <Form.Label>По-батькові</Form.Label>
                    <Form.Control type="text" placeholder="По-батькові" {...register('middleName')} />
                </Form.Group>

                <Form.Group controlId="lastName">
                    <Form.Label>Прізвище</Form.Label>
                    <Form.Control type="text" placeholder="Прізвище" {...register('lastName')} />
                </Form.Group>

                <Form.Group controlId="shortName">
                    <Form.Label>Коротке ім'я</Form.Label>
                    <Form.Control type="text" placeholder="Коротке ім'я" {...register('shortName')} />
                </Form.Group>

                <Form.Group controlId="nameGenitive">
                    <Form.Label>Ім'я у родовому</Form.Label>
                    <Form.Control type="text" placeholder="Ім'я у родовому" {...register('nameGenitive')} />
                </Form.Group>

                <Form.Group controlId="organizationUnitId">
                    <input type='hidden' {...register('organizationUnitId')} value={selectedUnit}/>
                    <Form.Label>Підрозділ</Form.Label>
                    <SelectList register='organizationUnitId' id={'organizationUnitId'} items={units ?? []} />
                </Form.Group>

                <Form.Group controlId="position">
                    <Form.Label>Посада</Form.Label>
                    <SelectList register='positionId' id={'position'} items={mapPositionsToIdNamePair(positions, selectedUnit)}
                                disabled={selectedUnit === undefined}/>
                </Form.Group>

                <Form.Group controlId="regimes">
                    <Form.Label>Посада</Form.Label>
                    <SelectList register='regimeId' id={'regimes'} items={regimes ?? []}
                                disabled={selectedUnit === undefined}/>
                </Form.Group>

                <Form.Group controlId="dateFrom">
                    <Form.Label>Дата прийняття</Form.Label>
                    <Form.Control type="date" placeholder="Ім'я" {...register('dateFrom')} />
                </Form.Group>

                <Form.Group controlId="dateTo">
                    <Form.Label>Дата звільнення</Form.Label>
                    <Form.Control type="date" placeholder="Ім'я" {...register('dateTo')} />
                </Form.Group>

                <Form.Group controlId="sex">
                    <Form.Label>Стать</Form.Label>
                    <SelectList register='sex' id="sex" items={sexList}/>
                </Form.Group>

                <Form.Group controlId="marriedStatus">
                    <Form.Label>Сімейний стан</Form.Label>
                    <SelectList register='marriedStatus' id="marriedStatus" items={marriedStatusList}/>
                </Form.Group>

                <Form.Group controlId="benefits">
                    <Form.Label>Пільги</Form.Label>
                    <SelectList register='benefits' id="benefits" items={benefitsList} multiple={true}/>
                </Form.Group>

                <line/>
                <h4>Банківський рахунок</h4>
                <Form.Group controlId="bankAccount.name">
                    <Form.Label>Назва</Form.Label>
                    <Form.Control type='text' placeholder='Введіть назву банку' {...register('bankAccount.name')}/>
                </Form.Group>

                <Form.Group controlId="bankAccount.account">
                    <Form.Label>Назва</Form.Label>
                    <Form.Control type='text' placeholder='Введіть назву банку' {...register('bankAccount.account')}/>
                </Form.Group>

                <Form.Group controlId="bankAccount.iban">
                    <Form.Label>Назва</Form.Label>
                    <Form.Control type='text' placeholder='Введіть назву банку' {...register('bankAccount.iban')}/>
                </Form.Group>

                <Form.Group controlId="bankAccount.mfo">
                    <Form.Label>Назва</Form.Label>
                    <Form.Control type='text' placeholder='Введіть назву банку' {...register('bankAccount.mfo')}/>
                </Form.Group>

                <line/>
                <h4>Контакти</h4>
                <Form.Group controlId="phone">
                    <Form.Label>Телефон</Form.Label>
                    <Form.Control type="phone" {...register('phone')} />
                </Form.Group>
                <Form.Group controlId="email">
                    <Form.Label>Телефон</Form.Label>
                    <Form.Control type="email" {...register('email')} />
                </Form.Group>
                <Form.Group controlId="telegram">
                    <Form.Label>Телефон</Form.Label>
                    <Form.Control type="text" {...register('telegram')} />
                </Form.Group>

                <Button type='submit' variant='primary' className='mt-3'>Зберегти</Button>
            </Form>
        </Container>
    );
}