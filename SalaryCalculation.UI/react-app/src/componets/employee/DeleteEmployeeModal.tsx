import React, {useState} from "react";
import RestUnitOfWork from "../../store/rest/RestUnitOfWork";
import {useForm} from "react-hook-form";
import {CustomModalDialog} from "../helpers/CustomModalDialog";

const DeleteEmployeeModal : React.FC<{deleteId: number | undefined, show: boolean, setShow: (show: boolean) => void}> = ({
    deleteId, show, setShow }) => {
    const restClient = new RestUnitOfWork();
    const {
        register,
    } = useForm<{id: number}>()

    const handleDelete = (id: number) => {
        restClient.organization.deleteEmployeeAsync(id)
            .then(response => {
                if(response)
                    console.log('employee was deleted');
                else console.error('employee was not deleted');
            });
    }

    const modalBody = [
        {id: 'id', control: <input type='hidden' {...register('id')} value={deleteId}/> }
    ]

    return (
        <CustomModalDialog show={show} handleChangeShow={(show) => setShow(show)}
                           headerText='Видалити працівника'
                           body={modalBody} id='delete-modal'
                           handleActionBtn={(data) => handleDelete(data.id)}
        footer={{actionBtnStyle: 'danger', actionBtnText: 'Видалити'}}/>
    );
}

export default DeleteEmployeeModal;