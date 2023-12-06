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