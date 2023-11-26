import {Component} from "react";
import RestApiClient, {RestApiProps} from "./RestApiClient";
import {IdNamePair} from "../../models/BaseModels";
import {EmployeeSearchCommand} from "../commands/OrganizationCommands";
import {EmployeeDto, OrganizationDto, OrganizationUnitDto, PositionDto} from "../../models/DTO";


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


    async getOrganizationAsync(organizationId: number): Promise<OrganizationDto> {
        const response = await this.apiClient.getAsync(`/api/organizations/${organizationId}`);
        let organization: OrganizationDto = {} as OrganizationDto;
        if (response.isSuccess)
            organization = this.apiClient.mapData<OrganizationDto>(response) as OrganizationDto;
        return organization;
    }

    async getOrganizationsShortAsync(): Promise<IdNamePair[]> {
        const response = await this.apiClient.getAsync('/api/organizations/all/short');
        let organizations: IdNamePair[] = [];
        if (response.isSuccess)
            organizations = this.apiClient.mapData<IdNamePair[]>(response) as IdNamePair[];
        return organizations;
    }

    async getOrganizationUnitsAsync(organizationId: number) : Promise<OrganizationUnitDto[]> {
        const response = await this.apiClient.getAsync(`/api/organizations/${organizationId}/units`);
        let units: OrganizationUnitDto[] = [];
        if(response.isSuccess)
            units = this.apiClient.mapData<OrganizationUnitDto[]>(response) as OrganizationUnitDto[];
        return units;
    }

    async getOrganizationUnitsShortAsync(organizationId: number) : Promise<IdNamePair[]> {
        const response = await this.apiClient.getAsync(`/api/organizations/${organizationId}/units/short`);
        let units: IdNamePair[] = [];
        if(response.isSuccess)
            units = this.apiClient.mapData<IdNamePair[]>(response) as IdNamePair[];
        return units;
    }

    async getPositionsAsync(organizationId: number, organizationUnitId : number | undefined = undefined) : Promise<PositionDto[]> {
        let url = `/api/organizations/${organizationId}/positions`;
        if(organizationUnitId)
            url += `/${organizationUnitId}`;
        const response = await this.apiClient.getAsync(url);
        let positions: PositionDto[] = [];
        if(response.isSuccess)
            positions = this.apiClient.mapData<PositionDto[]>(response) as PositionDto[];
        return positions;
    }

    async getPositionsShortAsync(organizationId: number, organizationUnitId : number | undefined = undefined) : Promise<IdNamePair[]> {
        let url = `/api/organizations/${organizationId}/positions/short`;
        if(organizationUnitId)
            url += `/${organizationUnitId}`;
        const response = await this.apiClient.getAsync(url);
        let positions: IdNamePair[] = [];
        if(response.isSuccess)
            positions = this.apiClient.mapData<IdNamePair[]>(response) as IdNamePair[];
        return positions;
    }

    async searchEmployeesAsync(cmd : EmployeeSearchCommand) : Promise<EmployeeDto[]> {
        const response = await this.apiClient.postAsync('/api/employees/search', cmd);
        let employees: EmployeeDto[] = [];
        if(response.isSuccess)
            employees = this.apiClient.mapData<EmployeeDto[]>(response) as EmployeeDto[];
        return employees;
    }

    async createOrganizationUnitAsync(unit: OrganizationUnitDto) : Promise<void> {
        const cmd = {
            organizationId: unit.organizationId,
            name: unit.name,
            organizationUnitId: unit.organizationUnitId
        };
        await this.apiClient.postAsync('/api/organizations/units/create', cmd);
    }

    async updateOrganizationUnitAsync(unit: OrganizationUnitDto) : Promise<void> {
        const cmd = {
            id: unit.id,
            organizationId: unit.organizationId,
            name: unit.name,
            organizationUnitId: unit.organizationUnitId
        };
        await this.apiClient.putAsync(`/api/organizations/units/update/${unit.id}`, cmd);
    }

    async deleteOrganizationUnitAsync(organizationId: number, id: number) : Promise<void> {
        await this.apiClient.deleteAsync(`/api/organizations/${organizationId}/units/delete/${id}`);
    }

    async createPositionAsync(position: PositionDto) : Promise<void> {
        const cmd = {
            organizationId: position.organizationId,
            name: position.name,
            organizationUnitId: position.organizationUnitId
        };
        await this.apiClient.postAsync('/api/organizations/positions/create', cmd);
    }

    async updatePositionAsync(position: PositionDto) : Promise<void> {
        const cmd = {
            id: position.id,
            organizationId: position.organizationId,
            name: position.name,
            organizationUnitId: position.organizationUnitId
        };
        await this.apiClient.putAsync(`/api/organizations/positions/update/${position.id}`, cmd);
    }

    async deletePositionAsync(organizationId: number, organizationUnitId: number, id: number) : Promise<void> {
        await this.apiClient.deleteAsync(`/api/organizations/${organizationId}/units/${organizationUnitId}/positions/delete/${id}`);
    }
}