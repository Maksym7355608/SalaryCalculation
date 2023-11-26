import React from "react";
import $ from 'jquery';
import axios, {AxiosHeaders, AxiosRequestConfig, AxiosResponse} from 'axios';

export interface RestApiProps {
    baseUrl: string;
    token: string | undefined;
}

interface RestResponse {
    isSuccess: boolean;
    data: any;
    errors: string[] | null;
}

class RestApiClient extends React.Component<RestApiProps> {
    private readonly config: AxiosRequestConfig;
    constructor(props: RestApiProps) {
        super(props);
        let headers = new AxiosHeaders();
        headers.set('Accept', '*/*');
        headers.set('Content-Type', 'application/json');
        if (this.props.token)
        {
            headers.set('Authorization', `Bearer ${this.props.token}`);
        }
        this.config = {
            baseURL: props.baseUrl,
            headers: headers,
            timeout: 30000,
            responseType: "json",
            timeoutErrorMessage: "timeout exception"
        };
    }

    // Метод для виконання GET-запиту
    async getAsync(url: string): Promise<RestResponse> {
        const response = await axios.get<RestResponse>(url, this.config);
        return this.handleResponse(response);
    }

    // Метод для виконання POST-запиту
    async postAsync<T>(url: string, data: T): Promise<RestResponse> {
        const response = await axios.post<RestResponse>(url, data, this.config);
        return this.handleResponse(response);
    }

    // Метод для виконання PUT-запиту
    async putAsync<T>(url: string, data: T, config = {}): Promise<RestResponse> {
        const response = await axios.put<RestResponse>(url, data, this.config);
        return this.handleResponse(response);
    }

    // Метод для виконання DELETE-запиту
    async deleteAsync(url: string, config = {}): Promise<RestResponse> {
        const response = await axios.delete<RestResponse>(url, this.config);
        return this.handleResponse(response);
    }

    private handleResponse(response : AxiosResponse<RestResponse, any>) : RestResponse{
        if(response.status != 200)
        {
            this.handleError("requestInvalid", 'Network response was not ok');
            return {isSuccess: false} as RestResponse;
        }
        const result = response.data;
        if(!result.isSuccess)
            this.handleError("responseInvalid", result.errors?.join(", ") as string);
        return result;
    }

    mapData<T>(response: RestResponse): T | null {
        if (response.data) {
            let data = response.data as T;
            if (data)
                return data;
            else
                return null;
        } else
            return null;
    }

    private handleError(key: string, errorMessage: string) {
        $(`#${key}`).text(errorMessage);
    }
}

export default RestApiClient;
