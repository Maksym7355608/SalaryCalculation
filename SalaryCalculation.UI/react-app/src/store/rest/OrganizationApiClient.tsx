import RestApiClient, {RestApiProps} from "./RestApiClient";
import {IdNamePair} from "../../models/BaseModels";
import {
    EmployeeCreateCommand,
    EmployeeSearchCommand,
    EmployeeUpdateCommand
} from "../../models/commands/OrganizationCommands";
import {EmployeeDto, OrganizationDto, OrganizationUnitDto, PositionDto} from "../../models/DTO";
import {EmployeeModel} from "../../models/employees";
import {EContactKind, EPermission} from "../../models/Enums";


export default class OrganizationApiClient {
    private readonly url = "http://localhost:5100";
    private readonly apiClient: RestApiClient;

    constructor() {
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

    async updateOrganizationPermissionsAsync(organizationId: number, permissions: EPermission[]): Promise<boolean> {
        const response = await this.apiClient.putAsync(`/api/organizations/${organizationId}/permissions/set`, {
            organizationId: organizationId,
            permissions: permissions
        });
        return response.isSuccess;
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

    async createOrganizationAsync(organization: OrganizationDto) {
        const response = await this.apiClient.postAsync(`/api/organizations/create`, {
            code: organization.code,
            name: organization.name,
            edrpou: organization.edrpou,
            address: organization.address,
            factAddress: organization.factAddress,
            bankAccounts: organization.bankAccounts,
            chief: organization.chief,
            accountant: organization.accountant,
            manager: organization.manager
        });
        return response.isSuccess;
    }

    async updateOrganizationAsync(organization: OrganizationDto) {
        const response = await this.apiClient.putAsync(`/api/organizations/update`, {
            id: organization.id,
            code: organization.code,
            name: organization.name,
            edrpou: organization.edrpou,
            address: organization.address,
            factAddress: organization.factAddress,
            bankAccounts: organization.bankAccounts,
            chief: organization.chief,
            accountant: organization.accountant,
            manager: organization.manager
        });
        return response.isSuccess;
    }

    async getEmployeeAsync(id: number) {
        const response = await this.apiClient.getAsync(`/api/employees/${id}`);
        const data = response.data as EmployeeDto;
        return this.mapEmployeeDtoToEmployeeModel(data);
    }

    async searchEmployeesAsync(cmd : EmployeeSearchCommand) {
        const response = await this.apiClient.postAsync('/api/employees/search', cmd);
        let employees: EmployeeDto[] = [];
        if(response.isSuccess)
            employees = this.apiClient.mapData<EmployeeDto[]>(response) as EmployeeDto[];
        return employees;
    }

    async createEmployeeAsync(employee: EmployeeCreateCommand) {
        const response = await this.apiClient.postAsync('/api/employees/create', employee);
        return response.isSuccess;
    }

    async updateEmployeeAsync(employee: EmployeeUpdateCommand) {
        const response = await this.apiClient.putAsync(`/api/employees/update/${employee.id}`, employee);
        return response.isSuccess;
    }

    async deleteEmployeeAsync(id: number) {
        const response = await this.apiClient.deleteAsync(`/api/employees/delete/${id}`);
        return response.isSuccess;
    }

    private mapEmployeeDtoToEmployeeModel(data: EmployeeDto) {
        return {
            id: data.id,
            rollNumber: data.rollNumber,
            dateFrom: data.dateFrom,
            dateTo: data.dateTo,
            dateFromSalary: data.salaries[data.salaries.length-1].dateFrom,
            salary: data.salaries[data.salaries.length-1].amount,
            benefits: data.benefits,
            sex: data.sex,
            marriedStatus: data.marriedStatus,
            bankAccount: data.bankAccount,
            phone: data.contacts.find(c => c.kind == EContactKind.phone)?.value,
            email: data.contacts.find(c => c.kind == EContactKind.email)?.value,
            telegram: data.contacts.find(c => c.kind == EContactKind.telegram)?.value,
            organizationId: data.organization.id,
            organizationUnitId: data.organizationUnit.id,
            positionId: data.position.id,
            regimeId: data.regime.id
        } as EmployeeModel;
    }
}