import React from "react";
import RestApiClient, {RestApiProps} from "./RestApiClient";
import {AuthentificateUserViewCommand, UserCreateCommand} from "./IdentityCommands/IdentityCommands";


class IdentityApiClient extends React.Component<RestApiProps, {apiClient : RestApiClient}>{
    constructor(props: RestApiProps) {
        super(props);
        this.setState({
            apiClient : new RestApiClient(props)
        });
    }

    async signInAsync(username : string, password : string) : Promise<string> {
        let result = await this.state.apiClient.postAsync<string, AuthentificateUserViewCommand>("api/identity", {username, password});
        return result as string;
    }

    async signUpAsync(command : UserCreateCommand) {
        let result = await this.state.apiClient.postWithoutDataAsync<UserCreateCommand>("api/identity/create", command);
    }

}