import {IdNamePair} from "../BaseModels";

export interface PaymentCard {
    id: number;
    organizationId: number;
    employee: IdNamePair;
    calculationDate: Date;
    paymentDate?: Date;
    calculationPeriod: number;
    payedAmount: number;
    accrualAmount: number;
    maintenanceAmount: number;
}

export interface Operation {
    id: number;
    code: number;
    name: string;
    amount: number;
    hours: number;
    note: string;
    period: number;
    employeeId: number;
    organizationId: number;
    sign: number;
}