import {useParams} from "react-router-dom";
import React, {useEffect, useState} from "react";
import {Button, Container, Form} from "react-bootstrap";
import {OrganizationDto, BankDto} from "../../models/DTO";
import RestUnitOfWork from "../../store/rest/RestUnitOfWork";
import {SubmitHandler, useForm, useFieldArray} from "react-hook-form";

export const Organization = () => {
    const [organization, setOrganization] = useState<OrganizationDto | undefined>(undefined)
    const [isEditMode, setIsEditMode] = useState(false);
    const [isBankRerender, setRerender] = useState(false);

    const { id } = useParams();
    const restClient = new RestUnitOfWork();
    const {register, control, handleSubmit} = useForm<OrganizationDto>()
    const { fields, append, remove } = useFieldArray({
        control,
        name: 'bankAccounts', // Назва вашого масиву в формі
    });

    const onSubmit: SubmitHandler<OrganizationDto> = (data) => isEditMode ? handleUpdate(data) : handleCreate(data);

    useEffect(() => {
        if (id !== 'new') {
            restClient.organization.getOrganizationAsync(parseInt(id as string))
                .then(result => {
                    setOrganization(result);
                });
            setIsEditMode(true);
        }
    }, []);

    const handleCreate = (data: OrganizationDto) => {
        restClient.organization.createOrganizationAsync(data)
            .then(response => {
                if(response)
                    console.log("organization successfully created");
                else
                    console.error("organization was not created")
            });
    }

    const handleUpdate = (data: OrganizationDto) => {
        restClient.organization.updateOrganizationAsync(data)
            .then(response => {
                if(response)
                    console.log("organization successfully updated");
                else
                    console.error("organization was not updated")
            });
    }

    const BankDetailsForm: React.FC<{ bankAccount: BankDto, index: number }> = ({ bankAccount, index }) => {
        return (
            <div key={index}>
                <div className="row input-group mt-2">
                    <Form.Label className="col-2">Назва</Form.Label>
                    <Form.Control className="col-3" type="text"
                                  placeholder="Bank Name" {...register(`bankAccounts.${index}.name` as const)}
                                    defaultValue={bankAccount.name}/>
                </div>
                <div className="row input-group mt-2">
                    <Form.Label className="col-2">Рахунок</Form.Label>
                    <Form.Control className="col-3" type="text"
                                  placeholder="Account" {...register(`bankAccounts.${index}.account` as const)}
                                  defaultValue={bankAccount.account} />
                </div>
                <div className="row input-group mt-2">
                    <Form.Label className="col-2">IBAN</Form.Label>
                    <Form.Control className="col-3" type="text"
                                  placeholder="IBAN" {...register(`bankAccounts.${index}.iban` as const)}
                                  defaultValue={bankAccount.iban} />
                </div>
                <div className="row input-group mt-2">
                    <Form.Label className="col-2">МФО</Form.Label>
                    <Form.Control className="col-3" type="text"
                                  placeholder="MFO" {...register(`bankAccounts.${index}.mfo` as const)}
                                  defaultValue={bankAccount.mfo} />
                </div>
                <div className="d-flex justify-content-end me-3 mt-1">
                    <Button type="button" variant="warning" size='sm' onClick={() => remove(index)}>
                        Видалити банк
                    </Button>
                </div>

            </div>
        )
    }
    
    return (
        <Container fluid>
            <h2>{isEditMode ? "Редагування організації" : "Створення організації"}</h2>
            <Form onSubmit={handleSubmit(onSubmit)}>
                {isEditMode && <Form.Control {...register('id')} hidden value={organization?.id}/>}
                <div className="row input-group mt-4">
                    <Form.Label className="col-2">Назва</Form.Label>
                    <Form.Control className="col-3" {...register('name')} disabled={isEditMode} defaultValue={organization?.name}/>
                </div>
                <div className="row input-group mt-2">
                    <Form.Label className="col-2">Код</Form.Label>
                    <Form.Control className="col-3" {...register('code')} disabled={isEditMode} defaultValue={organization?.code}/>
                </div>
                <div className="row input-group mt-2">
                    <Form.Label className="col-2">ЄДРПОУ</Form.Label>
                    <Form.Control className="col-3" {...register('edrpou')} defaultValue={organization?.edrpou}/>
                </div>
                <div className="row input-group mt-2">
                    <Form.Label className="col-2">Адреса</Form.Label>
                    <Form.Control className="col-3" {...register('address')} defaultValue={organization?.address}/>
                </div>
                <div className="row input-group mt-2">
                    <Form.Label className="col-2">Фактична адреса</Form.Label>
                    <Form.Control className="col-3" {...register('factAddress')} defaultValue={organization?.factAddress}/>
                </div>
                <div className="row input-group mt-2">
                    <Form.Label className="col-2">Власник</Form.Label>
                    <Form.Control className="col-3" {...register('chief')} defaultValue={organization?.chief}/>
                </div>
                <div className="row input-group mt-2">
                    <Form.Label className="col-2">Бухгалтер</Form.Label>
                    <Form.Control className="col-3" {...register('accountant')} defaultValue={organization?.accountant}/>
                </div>
                <line/>
                <h4>Банківські рахунки</h4>
                {
                    !isBankRerender && organization?.bankAccounts.map((item, index) => (
                        <BankDetailsForm bankAccount={item} index={index}/>
                    ))
                }
                {
                    fields.map((item, index) => (
                        <BankDetailsForm bankAccount={item} index={index + (organization?.bankAccounts.length ?? 0)}/>
                    ))
                }
                <div className="form-group mb-3 mt-2">
                    <Button variant="secondary" type="button" onClick={() => {
                        append({} as BankDto);
                        setRerender(true);
                    }}>
                        Додати банк
                    </Button>
                </div>


                <Button variant="primary" type="submit">Зберегти</Button>
            </Form>
        </Container>
    );
};