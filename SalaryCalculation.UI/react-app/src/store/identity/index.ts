import IdentityApiClient from "../rest/IdentityApiClient";
import {LoginForm, SignUpForm} from "../../models/identity/identity";

export const logIn = async (data: LoginForm) => {
    const identityApiClient = new IdentityApiClient();
    return await identityApiClient.signInAsync(data.username, data.password)
}

export const signUp = async (data: SignUpForm) => {
    const identityRestClient = new IdentityApiClient();
    return await identityRestClient.signUpAsync({
        username: data.username,
        password: data.password,
        firstName: data.firstName,
        middleName: data.middleName,
        lastName: data.lastName,
        email: data.email,
        phoneNumber: data.phone,
        organizationId: data.organization
    })
};

export const logOut = () => {
    localStorage.clear();
    window.location.href = '/login';
}