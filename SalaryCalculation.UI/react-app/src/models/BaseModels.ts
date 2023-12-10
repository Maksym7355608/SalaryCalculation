import {EPermission} from "./Enums";

export interface IdNamePairParametrized<T, U> {
    id: T;
    name: U
}

export interface IdNamePair {
    id: number;
    name: string;
}

export interface UserModel {
    token: string;
    id: string;
    organization: number;
    firstName: string;
    middleName: string;
    lastName: string;
    roles: IdNamePairParametrized<string, string>[];
    permissions: EPermission[];
}