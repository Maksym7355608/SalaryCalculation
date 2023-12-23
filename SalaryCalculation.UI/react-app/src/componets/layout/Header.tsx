import React from "react";
import {useSelector} from "react-redux";
import {logOut} from "../../store/identity";

const Header : React.FC<{title: string}> = ({ title }) => {
    const user = useSelector((state : any) => state.user);
    return (
        <nav className="navbar navbar-center mb-3">
            <div className="row w-100">
                <div className="col-3 p-input-icon-right">
                    <i className='material-icons small'>search</i>
                    <input type="text" className="form-control text-end" placeholder="Пошук "/>
                </div>
                <div className="col-5 text-lg-center nav-page-name">{title}</div>
                <div className="col-2 text-end nav-user">{user.firstName} {user.lastName} <span
                    className="material-icons">{"person"}</span></div>
                <div className="col-1 text-end nav-localization">укр/eng</div>
                <div className="col-1 text-end">
                    <button className="nav-log-out" onClick={logOut}>Вийти</button>
                </div>
            </div>

        </nav>
    );
}

export default Header;