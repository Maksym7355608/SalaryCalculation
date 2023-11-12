import {Component} from "react";
import {Link} from "react-router-dom";
import 'bootstrap/dist/css/bootstrap.min.css';
import IdentityApiClient from "../../actions/IdentityApiClient";
import {RestApiProps} from "../../actions/RestApiClient";


const baseUrl = "http://localhost:5300"; //TODO: get valid url in config file
class Login extends Component {
    async logInAsync(event: any) {
        event.preventDefault();
        const username = event.target.username.value as string;
        const password = event.target.password.value as string;

        const identityApiClient = new IdentityApiClient({baseUrl: baseUrl} as RestApiProps);
        const token1 = await identityApiClient.signInAsync(username, password);
        localStorage.setItem("token", token1);
        window.location.reload();
    }

    render() {
        return (
            <div className="screen-auth">
                <div className="container">
                    <div className="text-end">укр/eng</div>
                    <div className="text-auth-1 pt-5 pb-2">Вхід у систему</div>
                    <form className="form-auth p-5">
                        <div className="form-group mb-2">
                            <label htmlFor="username" className="form-label text-auth-2">Логін</label>
                            <input type="text" id="username" className="form-control" placeholder="Введіть логін"/>
                        </div>
                        <div className="form-group mb-2">
                            <label htmlFor="password" className="form-label text-auth-2">Пароль</label>
                            <input type="password" id="password" className="form-control" placeholder="Введіть пароль"/>
                        </div>
                        <div className="btn-group w-100 mt-3 d-flex justify-content-between">
                            <div className="div-btn-login">
                                <button className="btn btn-primary" onSubmit={(event) => this.logInAsync(event)}>
                                    Вхід
                                </button>
                            </div>
                            <div className="div-link-signup">
                                <Link to="/signup" className="link-signup">Реєстрація</Link>
                            </div>
                        </div>
                    </form>
                </div>
            </div>
        );
    }
}

export default Login;