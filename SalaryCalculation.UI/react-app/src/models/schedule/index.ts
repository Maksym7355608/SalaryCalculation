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