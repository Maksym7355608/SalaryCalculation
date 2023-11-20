import {Component} from "react";
import {RestUnitOfWork} from "../actions/rest/RestUnitOfWork";
import {EPermission, UserModel} from "../models/BaseModels";

export default class BasePageModel<T = {}, U = {}> extends Component<T, U> {
    protected readonly token = localStorage.getItem('token') as string
    //protected readonly restClient : RestUnitOfWork = new RestUnitOfWork()
    protected user : UserModel = JSON.parse(localStorage.getItem('user') as string) as UserModel;
    constructor(props: T) {
        super(props);
    }

    protected hasPermission(permission: EPermission) : boolean {
        return this.user.permissions.includes(permission);
    }
}