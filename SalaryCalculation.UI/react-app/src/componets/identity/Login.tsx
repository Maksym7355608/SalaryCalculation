import {Component} from "react";
import {Link} from "react-router-dom";
import $ from 'jquery';
import 'bootstrap/dist/css/bootstrap.min.css';
import IdentityApiClient from "../../actions/rest/IdentityApiClient";

class Login extends Component {

    componentDidMount() {
        document.title = "Вхід";
    }

    logInAsync = (event : any) => {
        event.preventDefault();
        const username = $('#username').val() as string;
        const password = $('#password').val() as string;

        const identityApiClient = new IdentityApiClient();
        identityApiClient.signInAsync(username, password).then(res => {
            const userModel = res;
            localStorage.setItem("token", userModel.token);
            localStorage.setItem("user", JSON.stringify(userModel));
        }).finally(() => window.location.reload());
    }

    render() {
        return (
            <div className="screen-auth">
                <div className="container">
                    <div className="text-end">укр/eng</div>
                    <div className="text-auth-1 pt-5 pb-2">Вхід у систему</div>
                    <form className="form-auth p-5" onSubmit={(event) => this.logInAsync(event)}>
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
                                <button type="submit" className="btn btn-primary">
                                    Вхід
                                </button>
                            </div>
                            <div className="div-link-signup">
                                <Link to="/signup" className="link-signup">Реєстрація</Link>
                            </div>
                            <span id="requestInvalid" className="text-danger"></span>
                            <span id="responseInvalid" className="text-danger"></span>
                        </div>
                    </form>
                </div>
            </div>
        );
    }
}

export default Login;