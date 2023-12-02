import React from "react";
import {Route, Routes, useNavigate} from "react-router-dom";
import {InitMenu} from "../componets/menu/Menu";
import Layout from "../componets/layout/Layout";

export function PrivateRoute() {
    const navigate = useNavigate();
    if(!localStorage.getItem("token"))
    {
        navigate("/login");
        return null;
    }

    const menuItems = InitMenu()

    return (
        <Routes>
            {menuItems.map(item =>
                <Route key={item.id} path={item.ref} element={<Layout title={item.text}>item.page</Layout>}/>
            )}
        </Routes>
    );
}