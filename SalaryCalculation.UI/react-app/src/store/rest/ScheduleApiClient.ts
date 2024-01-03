import {Component} from "react";
import RestApiClient, {RestApiProps} from "./RestApiClient";
import {IdNamePair} from "../../models/BaseModels";
import {EmployeeWithSchedule} from "../../models/employees";
import {EmpDay} from "../../models/schedule";

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
}