import {EPermission, UserModel} from "../../models/BaseModels";

export const user = JSON.parse(localStorage.getItem('user') as string) as UserModel;

export const hasPermission = (permission : EPermission) => {
    return user.permissions.includes(permission);
}

export function handleError(id: string, message: string) {
    $('#' + id + '-validation').text(message);
}