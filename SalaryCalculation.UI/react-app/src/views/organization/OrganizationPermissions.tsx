import React, {useEffect, useState} from "react";
import {OrganizationDto} from "../../models/DTO";
import {Button, Form, Table} from "react-bootstrap";
import {SubmitHandler, useForm} from "react-hook-form";
import RestUnitOfWork from "../../store/rest/RestUnitOfWork";
import {enumToIdNamePair, user} from "../../store/actions";
import {EPermission} from "../../models/Enums";

const OrganizationPermissions = () => {
    const {register, handleSubmit} = useForm();
    const onSubmit: SubmitHandler<any> = (data: any) => handleSubmitUpdatePermissions(data);
    const restClient = new RestUnitOfWork();
    const permissions = enumToIdNamePair(EPermission);

    const [organization, setOrganization] = useState({} as OrganizationDto);
    useEffect(() => {
        restClient.organization.getOrganizationAsync(user().organization)
            .then(result => {
                setOrganization(result);
            });
    }, []);

    const handleSubmitUpdatePermissions = (data: any) => {
        const keys = Object.keys(data);
        const updated = Object.values(data).map((v, index) => {
            if(v)
                return parseInt(keys[index]);
        }).filter(d => d);
        restClient.organization.updateOrganizationPermissionsAsync(organization.id, updated as EPermission[])
            .then(response => {
                if(response)
                    console.log('permissions was updated');
                else
                    console.error(`permissions was not updated`)
            });
    }
    const currentPermissions = (organization?.permissions as number[]) ?? [];
    return (
        <Form onSubmit={handleSubmit(onSubmit)}>
            {
                permissions.map(permission =>
                    <div className="form-check mb-2">
                        <Form.Check defaultChecked={currentPermissions.some(x => x === (permission.id as EPermission))} {...register(`${permission.id}`)}/>
                        <Form.Label htmlFor={`${permission.id}`}>{permission.name}</Form.Label>
                    </div>
                )
            }
            <Button type="submit" variant="primary">Зберегти зміни</Button>
        </Form>
    );
}

export default OrganizationPermissions;