import {IdNamePair, UserModel} from "../../models/BaseModels";
import {EPermission} from "../../models/Enums";

export const user = () => {
    return JSON.parse(localStorage.getItem('user') as string) as UserModel;
}

export const hasPermission = (permission : EPermission) => {
    return user().permissions.includes(permission);
}

export function handleError(id: string, message: string) {
    $('#' + id + '-validation').text(message);
}

export function enumToIdNamePair(type: any, localizer?: Record<string, string>) : IdNamePair[] {
    let keys = Object.keys(type);
    keys = keys.slice(0, keys.length / 2);
    let values = Object.values(type) as string[];
    values = values.slice(0, values.length / 2);
    return keys.map((key, index) => {
        return {
            id: parseInt(key),
            name: localizer ? localizer[values[index]] : values[index]
        }
    });
}