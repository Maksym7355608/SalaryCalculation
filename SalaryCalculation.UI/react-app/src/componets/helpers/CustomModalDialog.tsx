import React from "react";
import {Button, Form, Modal} from "react-bootstrap";
import {SubmitHandler, useForm} from "react-hook-form";


export enum EModalType {
    create,
    edit,
    delete,
    info,
}
interface BodyConfig {
    id: string;
    label?: string;
    control: React.ReactNode;
    hidden?: boolean;
    groupClass?: string;
}

interface FooterConfig {
    enableCloseBtn?: boolean;
    hasActionBtn?: boolean;
    actionBtnStyle?: string;
    actionBtnText?: string;
}

interface AdditionalConfig {
    centered?: boolean;
    headerCloseBtn?: boolean;
    size?: 'sm' | 'lg' | 'xl';
    scrollable?: boolean;
}

interface CustomModalDialogProps {
    show: boolean;
    handleChangeShow: (show: boolean) => void;
    headerText: string;
    body: BodyConfig[];
    footer?: FooterConfig;
    id: string;
    headerClass?: string;
    handleActionBtn: (data : any) => void;
    config?: AdditionalConfig;
}

export const CustomModalDialog: React.FC<CustomModalDialogProps> = (
    {
        show,
        handleChangeShow,
        headerText,
        body,
        footer,
        id,
        headerClass,
        handleActionBtn,
        config,
    }) => {

    const {
        handleSubmit,
    } = useForm();
    const onSubmit: SubmitHandler<any> = (data : any) => handleActionBtn(data)

    const handleClose = () => {
        handleChangeShow(false);
    };

    const renderHeader = () => {
        return (
            <Modal.Header closeButton={config?.headerCloseBtn} className={headerClass}>
                {headerText}
            </Modal.Header>
        );
    };

    const renderBody = () => {
        if (!body || body.length === 0) return null;

        return (
            <Modal.Body>
                {body.map((el) => (
                    <Form.Group key={el.id} controlId={el.id} hidden={el.hidden} className={el.groupClass}>
                        {el.label && <Form.Label>{el.label}</Form.Label>}
                        {el.control}
                        <span id={`${el.id}-validation`} className='text-danger'></span>
                    </Form.Group>
                ))}
            </Modal.Body>
        );
    };

    const renderFooter = () => {
        const footerConfig = {
            enableCloseBtn: true,
            hasActionBtn: true,
            actionBtnStyle: 'primary',
            actionBtnText: 'Зберегти',
            ...footer,
        };

        return (
            <Modal.Footer>
                {footerConfig.enableCloseBtn && <Button variant="secondary" onClick={handleClose}>Закрити</Button>}
                {footerConfig.hasActionBtn && <Button variant={footerConfig.actionBtnStyle} type="submit">{footerConfig.actionBtnText}</Button>}
                <span id={`${id}-validation`} className='text-danger'></span>
            </Modal.Footer>
        );
    };

    return (
        <Modal centered={config?.centered} show={show} onHide={handleClose} id={id}>
            {renderHeader()}
            <Form onSubmit={handleSubmit(onSubmit)}>
                {renderBody()}
                {renderFooter()}
            </Form>
        </Modal>
    );
};