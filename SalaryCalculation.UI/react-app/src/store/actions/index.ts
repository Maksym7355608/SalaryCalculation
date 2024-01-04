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

export function toShortDateString(date?: Date) {
    if(!date)
        return undefined;
    const datestring = date.getFullYear() + "-" + ("0" + (date.getMonth() + 1)).slice(-2) + "-" +
        ("0" + date.getDate()).slice(-2)
    return datestring;
}

export function toPeriodString(date?: Date) {
    if(!date)
        return undefined;
    let parsed : string[] = toShortDateString(date)?.split('-') as string[];
    parsed.pop();
    return parsed.join('-');
}

export function getDaysByMonth(month: number, isLeap?: boolean) : string[] {
    let result: string[] = [];
    let daysCount: number;
    switch (month){
        case 1:
        case 3:
        case 5:
        case 7:
        case 8:
        case 10:
        case 12:
            daysCount = 31;
            break;
        case 4:
        case 6:
        case 9:
        case 11:
            daysCount = 30;
            break;
        case 2:
            if(isLeap)
                daysCount = 29;
            else
                daysCount = 28;
            break
        default: return [];
    }
    for (let i = 1; i <= daysCount; i++)
        result = [...result, i.toString()];
    return result;
}

export const monthDict = [
    "Січень",
    "Лютий",
    "Березень",
    "Квітень",
    "Травень",
    "Червень",
    "Липень",
    "Серпень",
    "Вересень",
    "Жовтень",
    "Листопад",
    "Грудень",
]