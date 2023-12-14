import {Component} from "react";
import RestApiClient, {RestApiProps} from "./RestApiClient";
import {IdNamePair} from "../../models/BaseModels";

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
}