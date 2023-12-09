import {EPermission, IdNamePair, UserModel} from "../../models/BaseModels";

export const user = JSON.parse(localStorage.getItem('user') as string) as UserModel;

export const hasPermission = (permission : EPermission) => {
    return user.permissions.includes(permission);
}

export function handleError(id: string, message: string) {
    $('#' + id + '-validation').text(message);
}

export function enumToIdNamePair(type: any, localizer?: Record<string, string>) : IdNamePair[] {
    return Object.keys(type).map(key => ({
        id: type[key],
        name: (localizer && localizer[type[key].toString()]) || type[key].toString(),
    }));
}