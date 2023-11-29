import BasePageModel from "../BasePageModel";
import React from "react";

class Header extends BasePageModel<any> {

    constructor(props: any) {
        super(props);
    }

    logOut = () => {
        localStorage.clear();
        window.location.href = '/login';
    }

    render() {
        return (
            <nav className="navbar navbar-center mb-3">
                <div className="row w-100">
                    <div className="col-3">
                        <input type="text" className="form-control text-end" placeholder="Пошук"/>
                    </div>
                        <div className="col-5 text-lg-center nav-page-name">{this.props.title}</div>
                        <div className="col-2 text-end nav-user">{this.user.firstName} {this.user.lastName} <span className="material-icons">{"person"}</span></div>
                        <div className="col-1 text-end nav-localization">укр/eng</div>
                        <div className="col-1 text-end">
                            <button className="nav-log-out" onClick={() => this.logOut()}>Вийти</button>
                        </div>
                </div>

            </nav>
        );
    }
}

export default Header;