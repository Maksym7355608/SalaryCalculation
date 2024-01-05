import RestApiClient, {RestApiProps} from "./RestApiClient";
import {PaymentCardSearchCommand} from "../../models/commands/CalculationCommand";
import {PaymentCard} from "../../models/calculation";

export class CalculationApiClient {
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
}