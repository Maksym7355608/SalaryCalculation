import {CustomModalDialog} from "../helpers/CustomModalDialog";
import React from "react";
import {SubmitHandler, useForm} from "react-hook-form";
import {handleError} from "../../store/actions";
import RestUnitOfWork from "../../store/rest/RestUnitOfWork";

interface ModalProps{
    id?: number;
    show: boolean;
    setShow: (show: boolean) => void;
}
export const DeletePaymentCardModal: React.FC<ModalProps> = ({id, show, setShow}) => {
    const {register, handleSubmit} = useForm();
    const restClient = new RestUnitOfWork();
    const submitHandler: SubmitHandler<any> = (data: any) => handleDelete(data);
    const handleDelete = (data: any) => {
        if(!data.id)
        {
            handleError('id', 'id is null');
            return;
        }
        restClient.calculation.deletePaymentCardAsync(data.id).then(res => {
            if(res)
            {
                console.log('success delete');
                setShow(false);
            }
            else
                console.error('error delete');
        })
    };
    return (
        <CustomModalDialog id='delete-card' show={show} handleChangeShow={(show) => setShow(show)}
                           headerText={'Видалити нарахування'}
                           body={[{id: 'id', control: <input type='hidden' {...register('id')} value={id}/>} ]}
                           handleActionBtn={handleSubmit(submitHandler)}/>
    );
};