import {Component} from "react";
import RestApiClient, {RestApiProps} from "./RestApiClient";
import {IdNamePair} from "../../models/BaseModels";
import {EmployeeWithSchedule} from "../../models/employees";
import {EmpDay, PeriodCalendar, RegimeModel} from "../../models/schedule";
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

    async getRegimesAsync(organizationId: number) {
        const response = await this.apiClient.getAsync(`/api/schedule/regime/by-organization/${organizationId}`)
        return response.data as RegimeModel[];
    }

    async getRegimesShortAsync(organizationId: number) : Promise<IdNamePair[]> {
        const response = await this.apiClient.getAsync(`/api/schedule/regime/by-organization/${organizationId}/short`)
        return response.data as IdNamePair[];
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

    async setWorkDaysByRegimeAsync(data: any) {
        const response = await this.apiClient.postAsync('/api/schedule/calendar/calculate/day/mass', data);
        return response.isSuccess;
    }

    async updateWorkDaysAsync(cmd: any) {
        const response = await this.apiClient.postAsync('/api/schedule/calendar/day/set', cmd);
        return response.isSuccess;
    }

    async calculatePeriodCalendarAsync(period: number, employeeId: number) {
        const response = await this.apiClient.getAsync(`/api/schedule/calendar/calculate/period/${period}/employee/${employeeId}`);
        return response.isSuccess;
    }
}