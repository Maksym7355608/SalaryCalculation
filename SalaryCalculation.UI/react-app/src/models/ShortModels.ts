export interface EmployeeShortModel {
    id : number;
    rollNumber: string;
    fullName: string;
    unit: string;
    position: string;
    employeeDate : string;
    dismissDate : string;
    salary: number;
    sex: string;
    familyStatus: string;
    kids: number | undefined;
    contacts: string;
}