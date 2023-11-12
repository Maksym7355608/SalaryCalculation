export interface AuthentificateUserViewCommand {
    login: string;
    password : string;
}

export interface UserCreateCommand {
    username : string;
    password : string;
    firstName : string;
    middleName : string;
    lastName : string;
    phoneNumber : string;
    email : string;
    organizationId: number;
}