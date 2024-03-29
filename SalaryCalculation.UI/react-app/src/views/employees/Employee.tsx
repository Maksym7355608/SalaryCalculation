import {useNavigate, useParams, useSearchParams} from "react-router-dom";
import RestUnitOfWork from "../../store/rest/RestUnitOfWork";
import {SubmitHandler, useForm} from "react-hook-form";
import React, {useEffect, useState} from "react";
import {EmployeeModel} from "../../models/employees";
import {Button, Container, Form} from "react-bootstrap";
import {enumToIdNamePair, toShortDateString, user} from "../../store/actions";
import SelectList from "../../componets/helpers/SelectList";
import {IdNamePair} from "../../models/BaseModels";
import {PositionDto} from "../../models/DTO";
import {EBenefit, EMarriedStatus, ESex} from "../../models/Enums";
import {
    mapEmployeeModelToCreateCommand,
    mapEmployeeModelToUpdateCommand,
    mapPositionsToIdNamePair
} from "../../store/employees";
import DeleteEmployeeModal from "../../componets/employee/DeleteEmployeeModal";

export default function Employee() {
    const {id} = useParams();
    const navigate = useNavigate();
    const [searchParams, setSearchParams] = useSearchParams();
    const handler = searchParams.get('handler');
    const restClient = new RestUnitOfWork();
    const isEditMode = handler?.toLowerCase() == 'edit';
    const empId = parseInt(id as string);

    const [employee, setEmployee] = useState<EmployeeModel | undefined>(undefined);
    const [units, setUnits] = useState<IdNamePair[]>([]);
    const [positions, setPositions] = useState<PositionDto[]>([]);
    const [regimes, setRegimes] = useState<IdNamePair[]>([]);
    const [isInfoMode, setIsInfoMode] = useState(false);
    const sexList = enumToIdNamePair(ESex);
    const marriedStatusList = enumToIdNamePair(EMarriedStatus);
    const benefitsList = enumToIdNamePair(EBenefit);

    const [sUnit, setUnit] = useState<number>(0);
    const [sPosition, setPosition] = useState<number>(0);
    const [sRegime, setRegime] = useState<number>(0);
    const [sSex, setSex] = useState<number>(0);
    const [sStatus, setStatus] = useState<number>(0);
    const [sBenefits, setBenefits] = useState<number[]>([])


    const [deleteModal, setDeleteModel] = useState(false);

    const {register, control, handleSubmit} = useForm<EmployeeModel>();
    const onSubmit: SubmitHandler<EmployeeModel> = (data) => isEditMode ? handleUpdate(data) : handleCreate(data);

    useEffect(() => {
        const fetch = async () => {
            if (id && id !== 'create') {
                const emp = await restClient.organization.getEmployeeAsync(empId);
                setEmployee(emp);
                if(!isEditMode)
                    setIsInfoMode(true);
            }
            const shortUnits = await restClient.organization.getOrganizationUnitsShortAsync(user().organization);
            const pos = await restClient.organization.getPositionsAsync(user().organization);
            const shortRegimes = await restClient.schedule.getRegimesShortAsync(user().organization);

            setUnits(shortUnits);
            setUnit(shortUnits[0]?.id);
            setPositions(pos);
            setPosition(pos[0]?.id);
            setRegimes(shortRegimes);
            setRegime(shortRegimes[0].id);
        }

        fetch();
    }, [isEditMode]);

    const handleUpdate = (data: EmployeeModel) => {
        data.organizationUnitId = sUnit;
        data.positionId = sPosition;
        data.sex = sSex;
        data.marriedStatus = sStatus;
        data.benefits = sBenefits;
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
        data.organizationUnitId = sUnit;
        data.positionId = sPosition;
        data.sex = sSex;
        data.marriedStatus = sStatus;
        data.benefits = sBenefits;
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
            <h2>{isEditMode ? "Редагування працівника" : isInfoMode ? "Працівник" : "Додавання працівника"}</h2>
            <Form onSubmit={handleSubmit(onSubmit)}>
                <input type='hidden' {...register('organizationId')} value={user().organization}/>

                <Form.Group className="row input-group mt-4" controlId="rollNumber">
                    <Form.Label className="col-2">Табельний номер</Form.Label>
                    <Form.Control className="col-3" type="number"
                                  placeholder="Табельний номер" {...register('rollNumber')}
                    disabled={isInfoMode} defaultValue={employee?.rollNumber}/>
                </Form.Group>

                <Form.Group className="row input-group mt-2" controlId="firstName">
                    <Form.Label className="col-2">Ім'я</Form.Label>
                    <Form.Control className="col-3" placeholder="Ім'я" {...register('firstName')}
                                  disabled={isInfoMode} defaultValue={employee?.firstName} />
                </Form.Group>

                <Form.Group className="row input-group mt-2" controlId="middleName">
                    <Form.Label className="col-2">По-батькові</Form.Label>
                    <Form.Control className="col-3" type="text" placeholder="По-батькові" {...register('middleName')}
                                  disabled={isInfoMode} defaultValue={employee?.middleName} />
                </Form.Group>

                <Form.Group className="row input-group mt-2" controlId="lastName">
                    <Form.Label className="col-2">Прізвище</Form.Label>
                    <Form.Control className="col-3" type="text" placeholder="Прізвище" {...register('lastName')}
                                  disabled={isInfoMode} defaultValue={employee?.lastName} />
                </Form.Group>

                <Form.Group className="row input-group mt-2" controlId="shortName">
                    <Form.Label className="col-2">Коротке ім'я</Form.Label>
                    <Form.Control className="col-3" type="text" placeholder="Коротке ім'я" {...register('shortName')}
                                  disabled={isInfoMode} defaultValue={employee?.shortName}/>
                </Form.Group>

                <Form.Group className="row input-group mt-2" controlId="nameGenitive">
                    <Form.Label className="col-2">Ім'я у родовому</Form.Label>
                    <Form.Control className="col-3" type="text"
                                  placeholder="Ім'я у родовому" {...register('nameGenitive')}
                                  disabled={isInfoMode} defaultValue={employee?.nameGenitive}/>
                </Form.Group>

                <Form.Group className="row input-group mt-2" controlId="organizationUnitId">
                    <Form.Label className="col-2">Підрозділ</Form.Label>
                    <SelectList setState={(state) => setUnit(state as number)} id={'organizationUnitId'} items={units ?? []}
                                disabled={isInfoMode} value={employee?.organizationUnitId}/>
                </Form.Group>

                <Form.Group className="row input-group mt-2" controlId="position">
                    <Form.Label className="col-2">Посада</Form.Label>
                    <SelectList setState={(state) => setPosition(state as number)} id={'position'}
                                items={mapPositionsToIdNamePair(positions, sUnit)}
                                disabled={sUnit === undefined || isInfoMode} value={employee?.positionId}/>
                </Form.Group>

                <Form.Group className="row input-group mt-2" controlId="regimes">
                    <Form.Label className="col-2">Режим</Form.Label>
                    <SelectList setState={(state) => setRegime(state as number)} id={'regimes'} items={regimes}
                                disabled={sUnit === undefined || isInfoMode} value={sRegime}/>
                </Form.Group>

                <Form.Group className="row input-group mt-2">
                    <Form.Label className="col-2">Оклад</Form.Label>
                    <Form.Control className='col-3' type='text' placeholder='Оклад' {...register('salary')}
                                  disabled={isInfoMode} defaultValue={employee?.salary}/>
                </Form.Group>

                <Form.Group className="row input-group mt-2" controlId="dateFrom">
                    <Form.Label className="col-2">Дата прийняття</Form.Label>
                    <Form.Control className="col-3" type="date" placeholder="Ім'я" {...register('dateFrom')}
                                  disabled={isInfoMode || isEditMode} value={toShortDateString(employee?.dateFrom)}/>
                </Form.Group>

                <Form.Group className="row input-group mt-2" controlId="dateTo">
                    <Form.Label className="col-2">Дата звільнення</Form.Label>
                    <Form.Control className="col-3" type="date" placeholder="Ім'я" {...register('dateTo')}
                                  disabled={isInfoMode} defaultValue={toShortDateString(employee?.dateTo)}/>
                </Form.Group>

                <Form.Group className="row input-group mt-2" controlId="sex">
                    <Form.Label className="col-2">Стать</Form.Label>
                    <SelectList setState={(state) => setSex(state as number)} id="sex" items={sexList}
                                disabled={isInfoMode} value={employee?.sex}/>
                </Form.Group>

                <Form.Group className="row input-group mt-2" controlId="marriedStatus">
                    <Form.Label className="col-2">Сімейний стан</Form.Label>
                    <SelectList setState={(state) => setStatus(state as number)} id="marriedStatus" items={marriedStatusList}
                                disabled={isInfoMode} value={employee?.marriedStatus}/>
                </Form.Group>

                <Form.Group className="row input-group mt-2" controlId="benefits">
                    <Form.Label className="col-2">Пільги</Form.Label>
                     <div className='col-10 p-0' style={{height: '48px'}}>
                         <SelectList setState={(state) => setBenefits(state as number[])} id="benefits" items={benefitsList} multiple={true}
                                     disabled={isInfoMode} value={employee?.benefits}/>
                     </div>
                </Form.Group>

                <hr/>
                <h5>Банківський рахунок</h5>
                <Form.Group className="row input-group mt-2" controlId="bankAccount.name">
                    <Form.Label className="col-2">Назва</Form.Label>
                    <Form.Control className="col-3" type='text'
                                  placeholder='Введіть назву банку' {...register('bankAccount.name')}
                                  disabled={isInfoMode} defaultValue={employee?.bankAccount.name}/>
                </Form.Group>

                <Form.Group className="row input-group mt-2" controlId="bankAccount.account">
                    <Form.Label className="col-2">Рахунок</Form.Label>
                    <Form.Control className="col-3" type='text'
                                  placeholder='Введіть номер рахунку' {...register('bankAccount.account')}
                                  disabled={isInfoMode} defaultValue={employee?.bankAccount.account}/>
                </Form.Group>

                <Form.Group className="row input-group mt-2" controlId="bankAccount.iban">
                    <Form.Label className="col-2">IBAN</Form.Label>
                    <Form.Control className="col-3" type='text'
                                  placeholder='Введіть IBAN' {...register('bankAccount.iban')}
                                  disabled={isInfoMode} defaultValue={employee?.bankAccount.iban}/>
                </Form.Group>

                <Form.Group className="row input-group mt-2" controlId="bankAccount.mfo">
                    <Form.Label className="col-2">МФО</Form.Label>
                    <Form.Control className="col-3" type='text'
                                  placeholder='Введіть мфо' {...register('bankAccount.mfo')}
                                  disabled={isInfoMode} defaultValue={employee?.bankAccount.mfo}/>
                </Form.Group>

                <hr/>
                <h5>Контакти</h5>
                <Form.Group className="row input-group mt-2" controlId="phone">
                    <Form.Label className="col-2">Телефон</Form.Label>
                    <Form.Control className="col-3" type="phone" placeholder='+380*********' {...register('phone')}
                                  disabled={isInfoMode} defaultValue={employee?.phone}/>
                </Form.Group>
                <Form.Group className="row input-group mt-2" controlId="email">
                    <Form.Label className="col-2">Емейл</Form.Label>
                    <Form.Control className="col-3" type="email" placeholder='example@mail.com' {...register('email')}
                                  disabled={isInfoMode} defaultValue={employee?.email}/>
                </Form.Group>
                <Form.Group className="row input-group mt-2" controlId="telegram">
                    <Form.Label className="col-2">Телеграм</Form.Label>
                    <Form.Control className="col-3" type="text" placeholder='@username' {...register('telegram')}
                                  disabled={isInfoMode} defaultValue={employee?.telegram}/>
                </Form.Group>

                {!isInfoMode && <Button type='submit' variant='primary' className='mt-4'>Зберегти</Button>}
                {isInfoMode && (
                    <div className='row-gap-sm-0 mt-4'>
                        <Button className='me-2' type='button' variant='warning' onClick={() => {
                            navigate(`/employee/${empId}?handler=edit`);
                        }}>Редагування</Button>
                        <Button className='me-2' type='button' variant='danger' onClick={() => setDeleteModel(true)}>Видалити працівника</Button>
                        <Button className='me-2' type='button' variant='info'>Графік роботи</Button>
                        <Button className='me-2' type='button' variant='secondary'>Виписки</Button>
                        <DeleteEmployeeModal deleteId={empId} show={deleteModal} setShow={setDeleteModel}/>
                    </div>
                )}
            </Form>
        </Container>
    );
}