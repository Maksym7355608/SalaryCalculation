import {Component} from "react";
import RestApiClient, {RestApiProps} from "./RestApiClient";
import {IdNamePair} from "../../models/BaseModels";


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
}