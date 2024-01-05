import RestApiClient, {RestApiProps} from "./RestApiClient";
import {PaymentCardSearchCommand} from "../../models/commands/CalculationCommand";
import {Operation, PaymentCard} from "../../models/calculation";

export default class CalculationApiClient {
    private readonly url = "http://localhost:5217";
    private readonly apiClient: RestApiClient;

    constructor() {
        let settings = {baseUrl: this.url, token: undefined} as RestApiProps;
        const token = localStorage.getItem('token');
        if (token)
            settings.token = token as string;
        this.apiClient = new RestApiClient(settings);
    }

    async searchPaymentCardsAsync(cmd: PaymentCardSearchCommand) {
        const response = await this.apiClient.postAsync('/api/paymentCard/search', cmd);
        return response.data as PaymentCard[];
    }

    async deletePaymentCardAsync(id: number) {
        const response = await this.apiClient.deleteAsync(`/api/paymentCard/${id}`);
        return response.isSuccess;
    }

    async getPaymentCardAsync(id: number) {
        const response = await this.apiClient.getAsync(`/api/paymentCard/${id}`);
        return response.data as PaymentCard;
    }

    async getOperationsAsync(employeeId: number, period?: number) {
        const response = await this.apiClient.getAsync(`/api/operations/by-employee/${employeeId}/${period}`);
        return response.data as Operation[];
    }
}