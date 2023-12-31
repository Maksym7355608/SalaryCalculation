import {BankDto} from "../DTO";
import {EBenefit, EMarriedStatus, ESex} from "../Enums";
import {ScheduleShortModel} from "../schedule";

export interface EmployeeModel {
    id: number;
    rollNumber: number;
    firstName: string;
    lastName: string;
    middleName: string;
    shortName: string;
    nameGenitive: string;
    dateFrom: Date;
    dateTo?: Date;
    dateFromSalary: Date;
    salary: number;
    benefits: EBenefit[];
    sex: ESex;
    marriedStatus: EMarriedStatus;
    bankAccount: BankDto;
    phone?: string;
    email?: string;
    telegram?: string;
    organizationId: number;
    organizationUnitId: number;
    positionId: number;
    regimeId: number;
}

export interface EmployeeWithSchedule {
    id: number;
    name: string;
    period: number;
    schedule: ScheduleShortModel[];
}