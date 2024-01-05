import RestApiClient, {RestApiProps} from "./RestApiClient";
import {OperationData} from "../../models/dictionaries";

export default class DictionaryApiClient {
    private readonly url = "http://localhost:5400";
    private readonly apiClient: RestApiClient;

    constructor() {
        let settings = {baseUrl: this.url, token: undefined} as RestApiProps;
        const token = localStorage.getItem('token');
        if (token)
            settings.token = token as string;
        this.apiClient = new RestApiClient(settings);
    }

    async searchOperationDataAsync(data: any) {
        const response = await this.apiClient.postAsync('/api/dictionary/finance-data/search', data);
        return response.data as OperationData[];
    }
}