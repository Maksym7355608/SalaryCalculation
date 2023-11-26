import React, {Component, ReactNode} from "react";
import {Button, Form, Modal} from "react-bootstrap";

interface CustomModalDialogProps {
    id: string;
    headerText?: string;
    headerClass?: string;
    body?: BodyConfig[];
    footer?: FooterConfig;
    centered?: boolean;
    show: boolean;
    handleChangeShow(show: boolean): void;
    handleActionBtn(): void;
    config?: AdditionalConfig;
}

interface BodyConfig {
    id: string;
    label?: string;
    control: ReactNode;
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

export class CustomModalDialog extends Component<CustomModalDialogProps> {

    private readonly config = {
        centered: true,
        headerCloseBtn: true,
        size: undefined,
        scrollable: false,
    } as AdditionalConfig;

    constructor(props: CustomModalDialogProps) {
        super(props);
        if(props.config)
            this.config = $.extend(this.config, props.config);
    }

    private handleClose = () => {
        this.props.handleChangeShow(false);
    }

    private renderHeader() {
        return (
            <Modal.Header closeButton={this.config.headerCloseBtn} className={this.props.headerClass}>
                {this.props.headerText}
            </Modal.Header>
        );
    }

    private renderBody() {
        if(!this.props.body || this.props.body.length == 0)
            return undefined;

        return (
            <Modal.Body>
                {
                    this.props.body.map(el =>
                        el.label ?
                            <Form.Group key={el.id} controlId={el.id} hidden={el.hidden} className={el.groupClass}>
                                <Form.Label>{el.label}</Form.Label>
                                {el.control}
                                <span id={`${el.id}-validation`} className='text-danger'></span>
                            </Form.Group> :
                            <React.Fragment key={el.id}>
                                {el.control}
                                <span id={`${el.id}-validation`} className='text-danger'></span>
                            </React.Fragment>
                    )
                }
            </Modal.Body>
        );
    }

    private renderFooter() {
        const footerConfig = $.extend({
            enableCloseBtn: true,
            hasActionBtn: true,
            actionBtnStyle: 'primary',
            actionBtnText: 'Зберегти'
        } as FooterConfig, this.props.footer)
        return (
            <Modal.Footer>
                { footerConfig.enableCloseBtn && <Button variant="secondary" onClick={this.handleClose}>Закрити</Button> }
                { footerConfig.hasActionBtn &&
                    <Button variant={footerConfig.actionBtnStyle} type="submit">{footerConfig.actionBtnText}</Button>}
                <span id={`${this.props.id}-validation`} className='text-danger'></span>
            </Modal.Footer>
        );
    }

    render() {
        return (
            <Modal centered={this.config.centered} show={this.props.show} onHide={this.handleClose} id={this.props.id}>
                {this.renderHeader()}
                <Form onSubmit={this.props.handleActionBtn}>
                    {this.renderBody()}
                    {this.renderFooter()}
                </Form>
            </Modal>
        );
    }
}