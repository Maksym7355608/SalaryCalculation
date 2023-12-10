import React, {useEffect, useState} from "react";
import {OrganizationDto} from "../../models/DTO";
import {Table} from "react-bootstrap";
import {SubmitHandler, useForm} from "react-hook-form";
import RestUnitOfWork from "../../store/rest/RestUnitOfWork";
import {enumToIdNamePair, user} from "../../store/actions";
import {EPermission} from "../../models/Enums";

const OrganizationPermissions = () => {
    const {register, handleSubmit} = useForm<number[]>();
    const onSubmit: SubmitHandler<number[]> = (data: number[]) => handleSubmitUpdatePermissions(data);
    const restClient = new RestUnitOfWork();
    const permissions = enumToIdNamePair(EPermission);

    const [organization, setOrganization] = useState({} as OrganizationDto);
    useEffect(() => {
        restClient.organization.getOrganizationAsync(user().organization)
            .then(result => {
                setOrganization(result);
            });
    }, [restClient.organization]);

    const handleSubmitUpdatePermissions = (data: number[]) => {
        restClient.organization.updateOrganizationPermissionsAsync(organization.id, data)
            .then(response => {
                if(response)
                    console.log('permissions was updated');
                else
                    console.error(`permissions was not updated`)
            });
    }

    return (
        <form onSubmit={handleSubmit(onSubmit)}>
            <Table size='small'>
                <thead>
                <th></th>
                <th>Назва</th>
                </thead>
                <tbody>
                {
                    permissions.map(p => {
                        return (
                            <tr>
                                <td><input type='checkbox' checked={!!organization.permissions?.find(x => x == p.id)} value={p.id} {...register(`${p.id}`)}/></td>
                                <td>{p.name.toString()}</td>
                            </tr>
                        );
                    })
                }
                </tbody>
            </Table>
        </form>
    );
}

export default OrganizationPermissions;