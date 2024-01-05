export interface PaymentCardSearchCommand {
    id?: number;
    rollNumber: string;
    calculationPeriod?: any;
    organizationUnitId?: number;
    positionId?: number;
    organizationId: number;
}