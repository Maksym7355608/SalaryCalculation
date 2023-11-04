import axios, {AxiosInstance} from 'axios';
import React from "react";

interface RestApiProps {
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

        // Додайте перехоплювач запитів для додавання токена авторизації
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
    async get(url : string, config = {}) : Promise<RestResponse> {
        return (await this.state.apiClient.get<RestResponse>(url, config)).data;
    }

    // Метод для виконання POST-запиту
    async post<T>(url : string, data : T, config = {}) : Promise<RestResponse> {
        return (await this.state.apiClient.post<RestResponse>(url, data, config)).data;
    }

    // Метод для виконання PUT-запиту
    async put<T>(url : string, data : T, config = {}) : Promise<RestResponse> {
        return (await this.state.apiClient.put<RestResponse>(url, data, config)).data;
    }

    // Метод для виконання DELETE-запиту
    async delete(url : string, config = {}) : Promise<RestResponse> {
        return (await this.state.apiClient.delete<RestResponse>(url, config)).data;
    }
}

export default RestApiClient;
