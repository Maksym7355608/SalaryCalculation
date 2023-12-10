import RestApiClient, {RestApiProps} from "./RestApiClient";
import {UserCreateCommand} from "../../models/commands/IdentityCommands";
import {UserModel} from "../../models/BaseModels";

class IdentityApiClient {
    private readonly url = "http://localhost:5300";
    private readonly apiClient: RestApiClient;

    constructor() {
        let settings = {baseUrl: this.url, token: undefined} as RestApiProps;
        const token = localStorage.getItem('token');
        if (token)
            settings.token = token as string;
        this.apiClient = new RestApiClient(settings);
    }

    async signInAsync(login: string, password: string): Promise<UserModel> {
        const result = await this.apiClient.postAsync('/api/identity', {login, password});
        const userModel = {
            token: result.data.token,
            id: result.data.user.id,
            firstName: result.data.user.firstName,
            middleName: result.data.user.middleName,
            lastName: result.data.user.lastName,
            organization: result.data.user.organizationId,
            roles: result.data.user.roles,
            permissions: result.data.user.permissions
        } as UserModel;
        return userModel;
    }

    async signUpAsync(command: UserCreateCommand): Promise<boolean> {
        const result = await this.apiClient.postAsync("api/Identity/create", command);
        return result.isSuccess;
    }

}

export default IdentityApiClient;