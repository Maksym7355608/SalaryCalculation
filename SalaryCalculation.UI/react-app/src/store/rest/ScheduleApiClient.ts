import {Component} from "react";
import RestApiClient, {RestApiProps} from "./RestApiClient";
import {IdNamePair} from "../../models/BaseModels";
import {EmployeeWithSchedule} from "../../models/employees";
import {EmpDay, PeriodCalendar} from "../../models/schedule";
import {toPeriodString, toShortDateString} from "../actions";

export default class ScheduleApiClient extends Component {
    private readonly url = "http://localhost:5200";
    private readonly apiClient: RestApiClient;

    constructor() {
        super({});
        let settings = {baseUrl: this.url, token: undefined} as RestApiProps;
        const token = localStorage.getItem('token');
        if (token)
            settings.token = token as string;
        this.apiClient = new RestApiClient(settings);
    }

    async getRegimesShortAsync(organizationId: number) : Promise<IdNamePair[]> {
        return [];
    }

    async getScheduleShortAsync(data: any) {
        const response = await this.apiClient.postAsync(`/api/schedule/calendar/day/search`, data);
        return response.data as EmployeeWithSchedule[];
    }

    async getScheduleByEmployeeAsync(empId: number, period: number) {
        const response = await this.apiClient.getAsync(`/api/schedule/calendar/day/${empId}/${period}`);
        return response.data as EmpDay[];
    }

    async getCalendarAsync(empId: number, period: number) {
        const response = await this.apiClient.getAsync(`/api/schedule/calendar/period/${period}/${empId}`);
        const data = response.data;
        if(!data)
            return undefined;
        return {
            period: toPeriodString(new Date(data.period / 100, data.period % 100 - 1, 1)),
            workDays: data.workDays,
            hours: data.hours.summary,
            dayHours: data.hours.day,
            eveningHours: data.hours.evening,
            nightHours: data.hours.night,
            hoursH: data.hours.holidaySummary,
            dayHoursH: data.hours.holidayDay,
            eveningHoursH: data.hours.holidayEvening,
            nightHoursH: data.hours.holidayNight,
            vacation: data.vacationDays,
            sick: data.sickLeave,
            regime: data.regime,
        } as PeriodCalendar;
    }
}