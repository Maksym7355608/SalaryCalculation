import RestApiClient, {RestApiProps} from "./RestApiClient";
import {BaseAmount, Formula, OperationData} from "../../models/dictionaries";

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

    async searchOperationDataShortAsync(data: any) {
        const response = await this.searchOperationDataAsync(data);
        return response.map(op => {
            return {
                id: op.code,
                name: op.name + `(${op.code})`
            };
        });
    }

    async searchBaseAmountsAsync(data: any) {
        const response = await this.apiClient.postAsync('/api/dictionary/base-amount/search', data);
        return response.data as BaseAmount[];
    }

    async searchFormulasAsync(data: any) {
        const response = await this.apiClient.postAsync('/api/dictionary/formula/search', data);
        return response.data as Formula[];
    }

    async createFormulaAsync(data: any) {
        const response = await this.apiClient.postAsync('/api/dictionary/formula/create', data);
        return response;
    }

    async updateFormulaAsync(id: string, data: any) {
        const response = await this.apiClient.postAsync(`/api/dictionary/formula/update/${id}`, data);
        return response;
    }
}