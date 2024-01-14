import {IdNamePair} from "../BaseModels";

export interface ScheduleShortModel {
    day: string;
    work: string;
}

interface Time{
    hour: number;
    minutes: number;
}

export interface WorkDetail {
    daysOfWeek: string;
    startTime: Time;
    endTime: Time;
    isEndTimeNextDay: boolean;
    isHolidayWork: boolean;
    isHolidayShort?: boolean;
    startTimeInHoliday?: Time;
    endTimeInHoliday?: Time;
    isEndTimeInHolidayNextDay?: boolean;
    isLaunchPaid: boolean;
    launchTime?: number;
}

export interface RegimeModel {
    id: number;
    code: string;
    name: string;
    isCircle: boolean;
    workDays: WorkDetail[];
    restDays: string;
    startDateInCurrentYear: Date;
    startDateInPreviousYear?: Date;
    startDateInNextYear?: Date;
    shiftsCount: number;
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

export interface PeriodCalendar {
    period: string;
    workDays: number;
    hours: number;
    dayHours: number;
    eveningHours: number;
    nightHours: number;
    hoursH: number;
    dayHoursH: number;
    eveningHoursH: number;
    nightHoursH: number;
    vacation: number;
    sick: number;
    regime: IdNamePair;
}