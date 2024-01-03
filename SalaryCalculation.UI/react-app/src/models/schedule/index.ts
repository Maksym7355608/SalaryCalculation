export interface ScheduleShortModel {
    day: string;
    work: string;
}

interface WorkDetail {

}

export interface RegimeModel {
    id: number;
    code: string;
    name: string;
    isCircle: boolean;
    workDays: WorkDetail[]
}

export interface EmpDay {
    employeeId: number;
    date: string;
    day: number;
    evening: number;
    night: number;
    summary: string;
    holiday: boolean;
}