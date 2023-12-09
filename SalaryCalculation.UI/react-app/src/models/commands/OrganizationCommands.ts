import {EBenefit, EMarriedStatus, ESex} from "../Enums";
import {BankDto} from "../DTO";

export interface EmployeeSearchCommand {
    organizationId: number;
    rollNumber?: number;
    organizationUnitId?: number;
    positionId?: number;
    dateFrom?: Date;
    dateTo?: Date;
    salaryFrom?: number;
    salaryTo?: number;
}

interface EmployeeBaseCommand {
    firstName: string;
    lastName: string;
    middleName: string;
    shortName: string;
    nameGenitive: string;
    dateTo?: Date;
    salary: number;
    marriedStatus: EMarriedStatus;
    benefits: EBenefit[];
    bankAccount: BankDto;
    phone?: string;
    email?: string;
    telegram?: string;
    organizationUnitId: number;
    positionId: number;
    regimeId: number;
}

export interface EmployeeCreateCommand extends EmployeeBaseCommand {
    organizationId: number;
    rollNumber: number;
    dateFrom: Date;
    sex: ESex;
}

export interface EmployeeUpdateCommand extends EmployeeBaseCommand {
    id: number;
    dateSalaryUpdated?: Date;
    dateTo?: Date;
}