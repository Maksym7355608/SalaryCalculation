import {Component} from "react";
import {RestUnitOfWork} from "../actions/rest/RestUnitOfWork";
import {EPermission, UserModel} from "../models/BaseModels";

export default class BasePageModel<T = {}, U = {}> extends Component<T, U> {
    protected readonly token = localStorage.getItem('token') as string
    protected readonly restClient : RestUnitOfWork;
    protected user : UserModel = JSON.parse(localStorage.getItem('user') as string) as UserModel;
    constructor(props: T) {
        super(props);
        this.restClient = new RestUnitOfWork();
    }

    protected hasPermission(permission: EPermission) : boolean {
        return this.user.permissions.includes(permission);
    }

    protected handleError(id: string, message: string) {
        $('#' + id + '-validation').text(message);
    }

    protected clearErrors() {
        $('[id*="validation"]').text('');
        $('#requestInvalid').text('');
        $('#responseInvalid').text('');
    }
}