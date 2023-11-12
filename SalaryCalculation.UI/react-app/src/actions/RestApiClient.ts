import axios, {AxiosInstance} from 'axios';
import React from "react";

export interface RestApiProps {
    baseUrl : string;
    token : string | null;
}

interface RestResponse {
    isSuccess : boolean;
    data: any;
    errors: string[] | null;
}

class RestApiClient extends React.Component<RestApiProps, {headers: Headers}>{
    constructor(props: RestApiProps) {
        super(props);
        let headers = new Headers();
        headers.append('Content-Type', 'application/json');
        if (this.props.token)
            headers.append('Authorization', `Bearer ${this.props.token}`);
        this.state = {
            headers: headers
        };
    }

    // Метод для виконання GET-запиту
    async getAsync(url : string) : Promise<RestResponse> {
        let requestConfig = {
            method: 'GET',
            headers: this.state.headers,
        };
        const result = await this.baseRequestAsync(url, requestConfig);
        return result;
    }

    // Метод для виконання POST-запиту
    async postAsync<T>(url : string, data : T) : Promise<RestResponse> {
        let requestConfig = {
            method: 'POST',
            headers: this.state.headers,
            body: JSON.stringify(data)
        };
        const result = await this.baseRequestAsync(url, requestConfig);
        return result;
    }

    // Метод для виконання PUT-запиту
    async putAsync<T>(url : string, data : T, config = {}) : Promise<RestResponse> {
        let requestConfig = {
            method: 'PUT',
            headers: this.state.headers,
            body: JSON.stringify(data)
        };
        const result = await this.baseRequestAsync(url, requestConfig);
        return result;
    }

    // Метод для виконання DELETE-запиту
    async deleteAsync(url : string, config = {}) : Promise<RestResponse> {
        let requestConfig = {
            method: 'DELETE',
            headers: this.state.headers,
        };
        const result = await this.baseRequestAsync(url, requestConfig);
        return result;
    }

    private async baseRequestAsync(url: string, config: RequestInit) : Promise<RestResponse> {
        const result = await fetch(this.props.baseUrl + url, config)
            .then(response => response.json())
            .then((response) => {
                const res = response as RestResponse;
                if(!res.isSuccess)
                    this.handleError('responseInvalid', res.errors?.join(", ") as string)
                return response as RestResponse;
            },
                (errors) => {
                console.log(errors);
                this.handleError('requestInvalid', errors.errors.join(", ") as string);
                });
        return result as RestResponse;
    }

    mapData<T>(response : RestResponse) : T | null {
        if(response.data)
        {
            let data = response.data as T;
            if(data)
                return data;
            else
                return null;
        } else
            return null;
    }

    private handleError(key: string, errorMessage: string){

    }
}

export default RestApiClient;
