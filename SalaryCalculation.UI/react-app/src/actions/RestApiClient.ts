import axios, {AxiosInstance} from 'axios';
import React from "react";

export interface RestApiProps {
    baseUrl : string;
    token : string | null;
}

interface RestApiState {
 apiClient :  AxiosInstance;
}

interface RestResponse {
    isSuccess : boolean;
    data: any;
    errors: string[] | null;
}

class RestApiClient extends React.Component<RestApiProps, RestApiState>{
    constructor(props: RestApiProps) {
        super(props);
        const apiClient = axios.create({
            baseURL: props.baseUrl,
        });

        if (props.token)
            apiClient.interceptors.request.use((config) => {
                if (props.token) {
                    config.headers['Authorization'] = `Bearer ${props.token}`;
                }
                return config;
            });

        this.state = {
            apiClient,
        };
    }

    // Метод для виконання GET-запиту
    async getAsync<K>(url : string, config = {}) : Promise<K | null> {
        const response = (await this.state.apiClient.delete<RestResponse>(url, config)).data;
        if(response.isSuccess) {
            return this.mapData<K>(response.data);
        } else
            throw new Error(response.data.errors?.join(', ') || 'Помилка запиту')
    }

    // Метод для виконання POST-запиту
    async postAsync<K, T>(url : string, data : T, config = {}) : Promise<K | null> {
        const response = (await this.state.apiClient.delete<RestResponse>(url, config)).data;
        if(response.isSuccess) {
            return this.mapData<K>(response.data);
        } else
            throw new Error(response.data.errors?.join(', ') || 'Помилка запиту')
    }

    async postWithoutDataAsync<T>(url : string, data : T, config = {}) : Promise<void> {
        const response = (await this.state.apiClient.delete<RestResponse>(url, config)).data;
        if(!response.isSuccess) {
            throw new Error(response.data.errors?.join(', ') || 'Помилка запиту')
        }
    }

    // Метод для виконання PUT-запиту
    async putAsync<T>(url : string, data : T, config = {}) : Promise<boolean> {
        const response = (await this.state.apiClient.delete<RestResponse>(url, config)).data;
        if(response.isSuccess) {
            return response.isSuccess;
        } else
            throw new Error(response.data.errors?.join(', ') || 'Помилка запиту')
    }

    // Метод для виконання DELETE-запиту
    async deleteAsync(url : string, config = {}) : Promise<boolean> {
        const response = (await this.state.apiClient.delete<RestResponse>(url, config)).data;
        if(response.isSuccess) {
            return response.isSuccess;
        } else
            throw new Error(response.data.errors?.join(', ') || 'Помилка запиту')
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
}

export default RestApiClient;
