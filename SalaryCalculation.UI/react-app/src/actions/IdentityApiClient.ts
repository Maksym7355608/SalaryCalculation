import React from "react";
import RestApiClient, {RestApiProps} from "./RestApiClient";
import {UserCreateCommand} from "./IdentityCommands/IdentityCommands";

let apiClient : RestApiClient;

class IdentityApiClient extends React.Component<RestApiProps>{
    constructor(props: RestApiProps) {
        super(props);
        apiClient = new RestApiClient(props);
    }

    async signInAsync(login : string, password : string) : Promise<string> {
        const result = await apiClient.postAsync('/api/identity', {login, password});
        return result.data as string;
    }

    async signUpAsync(command : UserCreateCommand) : Promise<boolean> {
        const result = await apiClient.postAsync("api/Identity/create", command);
        return result.isSuccess;
    }

}

export default IdentityApiClient;