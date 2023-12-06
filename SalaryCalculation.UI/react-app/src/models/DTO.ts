import {EPermission, IdNamePair} from "./BaseModels";
import {EBenefit, EMarriedStatus, ESex} from "./Enums";

export interface EmployeeDto {
    id: number;
    rollNumber: number;
    name: PersonDto;
    dateFrom: Date;
    dateTo?: Date;
    salaries: SalaryDto[];
    benefits: EBenefit[];
    sex: ESex;
    marriedStatus: EMarriedStatus;
    bankAccount: BankDto;
    contacts: ContactDto[];
    organizationId: IdNamePair;
    organizationUnitId: IdNamePair;
    position: IdNamePair;
}

export interface SalaryDto {
    dateFrom: Date;
    dateTo?: Date;
    amount: number;
}

export interface PersonDto {
    firstName: string;
    lastName: string;
    middleName: string;
    shortName: string;
    nameGenitive: string;
}

export interface ContactDto {
    kind: number;
    value: string;
}

export interface BankDto {
    name: string;
    account: string;
    iban: string;
    mfo: number;
}

export interface OrganizationDto {
    id: number;
    code: string;
    name: string;
    edrpou: number;
    address: string;
    factAddress: string;
    bankAccounts: BankDto[];
    chief?: IdNamePair;
    accountant?: IdNamePair;
    manager?: IdNamePair;
    permissions: EPermission[];
}

export interface OrganizationUnitDto {
    id: number;
    name: string;
    organizationId: number;
    organizationUnitId?: number;
}

export interface PositionDto {
    id: number;
    name: string;
    organizationId: number;
    organizationUnitId: number;
}
