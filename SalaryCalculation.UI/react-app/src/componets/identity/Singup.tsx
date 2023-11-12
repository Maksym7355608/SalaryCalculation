import {Component} from "react";
import { Link, redirect  } from "react-router-dom";
import 'bootstrap/dist/css/bootstrap.min.css';
import IdentityApiClient from "../../actions/IdentityApiClient";
import RestApiClient, {RestApiProps} from "../../actions/RestApiClient";
import SelectList from "../../actions/helpers/SelectList";

interface SignupComponentState {
    loaded: boolean;
    organizations: IdNamePair[];
}

export interface IdNamePair{
    id: number;
    name: string;
}

const identityUrl = "http://localhost:5300"; //TODO: get valid url in config file
const organizationUrl = "http://localhost:5100";
class Signup extends Component<any, SignupComponentState> {

    constructor() {
        super({});
        this.state = {
            loaded: false,
            organizations: []
        }
    }
    async componentDidMount() {
        if(this.state.loaded)
            return;
        const organizationRestClient = new RestApiClient({baseUrl: organizationUrl} as RestApiProps);
        const orgResponse = await organizationRestClient.getAsync('/api/organizations/all/short');
        let organizations: IdNamePair[] = [];
        if (orgResponse.isSuccess)
            organizations = organizationRestClient.mapData<IdNamePair[]>(orgResponse) as IdNamePair[];
        this.setState({
            organizations: organizations,
            loaded: true
        });
    }
    async signUpAsync(event: any) : Promise<boolean> {
        event.preventDefault();
        const username = event.target.username.value as string;
        const password = event.target.password.value as string;
        const passwordConfirm = event.target.passwordConfirm.value as string;
        const firstName = event.target.firstName.value as string;
        const middleName = event.target.middleName.value as string;
        const lastName = event.target.lastName.value as string;
        const phone = event.target.phone.value as string;
        const email = event.target.email.value as string;
        const organization = event.target.organization.value as number;

        if(password != passwordConfirm)
            return false;

        const identityRestClient = new IdentityApiClient({baseUrl: organizationUrl} as RestApiProps);
        return await identityRestClient.signUpAsync({
            username: username,
            password: password,
            firstName: firstName,
            middleName: middleName,
            lastName: lastName,
            email: email,
            phoneNumber: phone,
            organizationId: organization
        });
    }
    render() {
        return (
            <div className="screen-auth">
                <div className="container">
                    <div className="text-end">укр/eng</div>
                    <div className="text-auth-1 pt-3 pb-2">Реєстрація</div>
                    <form className="form-auth p-3">
                        <div className="form-group mb-1">
                            <label htmlFor="username" className="form-label text-auth-2">Логін</label>
                            <input type="text" id="username" className="form-control" placeholder="Введіть логін"/>
                        </div>
                        <div className="form-group mb-1">
                            <label htmlFor="password" className="form-label text-auth-2">Пароль</label>
                            <input type="password" id="password" className="form-control" placeholder="Введіть пароль"/>
                        </div>
                        <div className="form-group mb-1">
                            <label htmlFor="passwordConfirm" className="form-label text-auth-2">Підтвердити пароль</label>
                            <input type="password" id="passwordConfirm" className="form-control" placeholder="Введіть пароль ще раз"/>
                        </div>
                        <div className="form-group mb-1">
                            <label htmlFor="firstName" className="form-label text-auth-2">Ім'я</label>
                            <input type="text" id="firstName" className="form-control" placeholder="Введіть ім'я"/>
                        </div>
                        <div className="form-group mb-1">
                            <label htmlFor="middleName" className="form-label text-auth-2">По-батькові</label>
                            <input type="text" id="middleName" className="form-control" placeholder="Введіть по-батькові"/>
                        </div>
                        <div className="form-group mb-1">
                            <label htmlFor="lastName" className="form-label text-auth-2">Прізвище</label>
                            <input type="text" id="lastName" className="form-control" placeholder="Введіть прізвище"/>
                        </div>
                        <div className="form-group mb-1">
                            <label htmlFor="phone" className="form-label text-auth-2">Телефон</label>
                            <input type="tel" id="phone" className="form-control" placeholder="Введіть номер телефону"/>
                        </div>
                        <div className="form-group mb-1">
                            <label htmlFor="email" className="form-label text-auth-2">Електронна адреса</label>
                            <input type="email" id="email" className="form-control" placeholder="Введіть електронну адреса"/>
                        </div>
                        <div className="form-group mb-1">
                            <label htmlFor="organization" className="form-label text-auth-2">Організація</label>
                            <SelectList useEmpty={false} emptyName={undefined} selectName="organization" items={this.state.organizations}/>
                        </div>
                        <div className="btn-group w-100 mt-2 d-flex justify-content-center">
                            <div className="div-btn-login">
                                <button className="btn btn-primary" onSubmit={(event) => this.signUpAsync(event)
                                    .then(r => r ? redirect('/login') : null)}>
                                    Реєстрація
                                </button>
                            </div>
                        </div>
                    </form>
                </div>
            </div>
        );
    }
}

export default Signup;