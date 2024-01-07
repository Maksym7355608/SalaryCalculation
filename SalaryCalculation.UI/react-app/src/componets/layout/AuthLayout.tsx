import {useNavigate} from "react-router-dom";
import React from "react";
import {LayoutProps} from "./Layout";

export const AuthLayout : React.FC<LayoutProps> = ({title, children}) => {
    const navigate = useNavigate();
    if(localStorage.getItem("token"))
    {
        navigate("/");
        return null;
    }
    document.title = title;
    return (
        <>{children}</>
    );
};