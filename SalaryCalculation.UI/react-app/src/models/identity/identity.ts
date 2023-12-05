import {IdNamePair} from "../BaseModels";

export interface LoginForm {
    username: string;
    password: string;
}

export interface SignupState {
    loaded: boolean;
    organizations: IdNamePair[];
}

export interface SignUpForm {
    username: string;
    password: string;
    passwordConfirm: string;
    firstName: string;
    middleName: string;
    lastName: string;
    phone: string;
    email: string;
    organization: number;
}