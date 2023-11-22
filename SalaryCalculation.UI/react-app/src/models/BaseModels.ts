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

export enum EPermission {
    roleControl = 1,
    organizationSettings = 2,
    searchEmployees = 3,
    createEmployees = 4,
    deleteEmployees = 5,
    viewSchedule = 6,
    searchSchedules = 7,
    calculateSchedules = 8,
    viewCalculation = 9,
    calculationSalaries = 10,
    viewDictionary = 11,
    createDocuments = 12,
}