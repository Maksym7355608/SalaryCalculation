import {Component} from "react";
import {RestUnitOfWork} from "../actions/rest/RestUnitOfWork";
import {EPermission, UserModel} from "../models/BaseModels";

export default class BasePageModel<T = {}, U = {}> extends Component<T, U> {
    protected readonly token = localStorage.getItem('token') as string
    protected readonly restClient : RestUnitOfWork;
    protected readonly user : UserModel;
    constructor(props: T) {
        super(props);
        this.token = localStorage.getItem('token') as string;
        //if(!this.token)
            //window.location.href = '/login'; Неясно що з цим робити
        this.user = JSON.parse(localStorage.getItem('user') as string) as UserModel;
        this.restClient = new RestUnitOfWork();
    }

    componentDidUpdate() {
        //this.clearErrors();
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