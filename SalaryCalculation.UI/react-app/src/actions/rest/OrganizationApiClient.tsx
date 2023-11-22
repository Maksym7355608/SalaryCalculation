import {Component} from "react";
import RestApiClient, {RestApiProps} from "./RestApiClient";
import {IdNamePair} from "../../models/BaseModels";
import {EmployeeShortModel} from "../../models/ShortModels";
import {EmployeeSearchCommand} from "../commands/OrganizationCommands";
import {EmployeeDto} from "../../models/DTO";


export class OrganizationApiClient extends Component {
    private readonly url = "http://localhost:5100";
    private readonly apiClient: RestApiClient;

    constructor() {
        super({});
        let settings = {baseUrl: this.url, token: undefined} as RestApiProps;
        const token = localStorage.getItem('token');
        if (token)
            settings.token = token as string;
        this.apiClient = new RestApiClient(settings);
    }

    async getOrganizationsShortAsync(): Promise<IdNamePair[]> {
        const response = await this.apiClient.getAsync('/api/organizations/all/short');
        let organizations: IdNamePair[] = [];
        if (response.isSuccess)
            organizations = this.apiClient.mapData<IdNamePair[]>(response) as IdNamePair[];
        return organizations;
    }

    async getOrganizationUnitsAsync(organizationId: number) {

    }

    async getOrganizationUnitsShortAsync(organizationId: number) : Promise<IdNamePair[]> {
        const response = await this.apiClient.getAsync(`/api/organizations/${organizationId}/units/short`);
        let units: IdNamePair[] = [];
        if(response.isSuccess)
            units = this.apiClient.mapData<IdNamePair[]>(response) as IdNamePair[];
        return units;
    }

    async getPositionsAsync(organizationId: number) {
    }

    async getPositionsShortAsync(organizationId: number, organizationUnitId : number | undefined = undefined) : Promise<IdNamePair[]> {
        let url = `/api/organizations/${organizationId}/positions/short`;
        if(organizationUnitId)
            url += `/${organizationUnitId}`;
        const response = await this.apiClient.getAsync(url);
        let units: IdNamePair[] = [];
        if(response.isSuccess)
            units = this.apiClient.mapData<IdNamePair[]>(response) as IdNamePair[];
        return units;
    }

    async searchEmployeesAsync(cmd : EmployeeSearchCommand) : Promise<EmployeeDto[]> {
        const response = await this.apiClient.postAsync('/api/employees/search', cmd);
        let employees: EmployeeDto[] = [];
        if(response.isSuccess)
            employees = this.apiClient.mapData<EmployeeDto[]>(response) as EmployeeDto[];
        return employees;
    }
}