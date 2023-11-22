export interface EmployeeSearchCommand {
    organizationId: number;
    rollNumber: number | null;
    organizationUnitId: number | null;
    positionId: number | null;
    dateFrom: Date | null;
    dateTo: Date | null;
    salaryFrom: number | null;
    salaryTo: number | null;
}