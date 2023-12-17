import React, {useState} from "react";
import RestUnitOfWork from "../../store/rest/RestUnitOfWork";
import {useForm} from "react-hook-form";
import {CustomModalDialog} from "../helpers/CustomModalDialog";

const DeleteEmployeeModal : React.FC<{deleteId: number | undefined, show: boolean}> = ({
    deleteId, show
                                                                                       }) => {
    const [showDelete, setShowDelete] = useState(show);
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
        <CustomModalDialog show={showDelete} handleChangeShow={(show) => setShowDelete(show)}
                           headerText='Видалити працівника'
                           body={modalBody} id='delete-modal'
                           handleActionBtn={(data) => handleDelete(data.id)}/>
    );
}

export default DeleteEmployeeModal;