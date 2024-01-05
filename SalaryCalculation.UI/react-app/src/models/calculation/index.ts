import {IdNamePair} from "../BaseModels";

export interface PaymentCard {
    id: number;
    organizationId: number;
    employee: IdNamePair;
    calculationDate: Date;
    paymentDate: Date | null;
    calculationPeriod: number;
    payedAmount: number;
    accrualAmount: number;
    maintenanceAmount: number;
}