import {useParams} from "react-router-dom";
import React, {useEffect, useState} from "react";
import {Container, Form} from "react-bootstrap";
import {OrganizationDto, BankDto} from "../../models/DTO";
import {RestUnitOfWork} from "../../store/rest/RestUnitOfWork";
import {SubmitHandler, useForm, useFieldArray} from "react-hook-form";

export const Organization = () => {
    const { id } = useParams();
    const restClient = new RestUnitOfWork();
    const {register, control, handleSubmit} = useForm<OrganizationDto>()
    const { fields, append, remove } = useFieldArray({
        control,
        name: 'bankAccounts', // Назва вашого масиву в формі
    });
    
    const [organization, setOrganization] = useState<OrganizationDto | undefined>(undefined)
    const [isEditMode, setIsEditMode] = useState(false);

    const onSubmit: SubmitHandler<OrganizationDto> = (data) => isEditMode ? handleUpdate(data) : handleCreate(data);

    useEffect(() => {
        if (id !== 'new') {
            restClient.organization.getOrganizationAsync(parseInt(id as string))
                .then(result => {
                    setOrganization(result);
                });
            setIsEditMode(true);
        }
    }, [id, restClient.organization]);

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
                <Form.Group controlId={`bankAccounts[${index}]name`}>
                    <Form.Label>Назва</Form.Label>
                    <Form.Control type="text"
                                  placeholder="Bank Name" {...register(`bankAccounts.${index}.name` as const)}
                                    defaultValue={bankAccount.name}/>
                </Form.Group>
                <Form.Group controlId={`bankAccounts[${index}]account`}>
                    <Form.Label>Рахунок</Form.Label>
                    <Form.Control type="text"
                                  placeholder="Account" {...register(`bankAccounts.${index}.account` as const)}
                                  defaultValue={bankAccount.account} />
                </Form.Group>
                <Form.Group controlId={`bankAccounts[${index}]iban`}>
                    <Form.Label>IBAN</Form.Label>
                    <Form.Control type="text"
                                  placeholder="IBAN" {...register(`bankAccounts.${index}.iban` as const)}
                                  defaultValue={bankAccount.iban} />
                </Form.Group>
                <Form.Group controlId={`bankAccounts[${index}]mfo`}>
                    <Form.Label>МФО</Form.Label>
                    <Form.Control type="text"
                                  placeholder="MFO" {...register(`bankAccounts.${index}.mfo` as const)}
                                  defaultValue={bankAccount.mfo} />
                </Form.Group>
                <button type="button" onClick={() => remove(index)}>
                    Видалити банк
                </button>
            </div>
        )
    }
    
    return (
        <Container fluid>
            <h2>{isEditMode ? "Редагування організації" : "Створення організації"}</h2>
            <Form onSubmit={handleSubmit(onSubmit)}>
                {isEditMode && <Form.Control {...register('id')} hidden value={organization?.id}/>}
                <Form.Group>
                    <Form.Label>Назва</Form.Label>
                    <Form.Control {...register('name')} disabled defaultValue={organization?.name}/>
                </Form.Group>
                <Form.Group className="form-group-inline">
                    <Form.Label>Код</Form.Label>
                    <Form.Control {...register('code')} disabled defaultValue={organization?.code}/>
                </Form.Group>
                <Form.Group>
                    <Form.Label>ЄДРПОУ</Form.Label>
                    <Form.Control {...register('edrpou')} disabled defaultValue={organization?.edrpou}/>
                </Form.Group>
                <Form.Group>
                    <Form.Label>Адреса</Form.Label>
                    <Form.Control {...register('address')} disabled defaultValue={organization?.address}/>
                </Form.Group>
                <Form.Group>
                    <Form.Label>Фактична адреса</Form.Label>
                    <Form.Control {...register('factAddress')} disabled defaultValue={organization?.factAddress}/>
                </Form.Group>
                <line/>
                <h3>Банківські рахунки</h3>
                {
                    fields.map((item, index) => (
                        <BankDetailsForm bankAccount={item} index={index}/>
                    ))
                }
                <button type="button" onClick={() => append({} as BankDto)}>
                    Додати банк
                </button>
            </Form>
        </Container>
    );
};