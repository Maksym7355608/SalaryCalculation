import React, {Component} from 'react';
import {BrowserRouter as Router, Navigate, Route, Routes} from "react-router-dom";
import './styles/App.css';
import './styles/Layout.css';

import Login from "./componets/identity/Login";
import Signup from "./componets/identity/Singup";
import Home from "./componets/home/Home";
import Menu from "./componets/home/Menu";
import {NotFound} from "./componets/notFound/NotFound";
import Layout from "./componets/home/Layout";
import {UserSettings} from "./componets/settings/UserSettings";
import {MainSettings} from "./componets/settings/MainSettings";

const isUserAuthenticated = !!localStorage.getItem('token');

class App extends Component {
    render() {
        let menu = new Menu({}).getMenuItemsWithLinks();
        return (
            <>
                <Router>
                    <Routes>
                        <Route path="/login" element={isUserAuthenticated ? <Navigate to="/"/> : <Login/>}/>
                        <Route path="/signup" element={<Signup/>}/>
                        <Route path="/" element={<Layout/>}>
                            <Route key={0} index element={isUserAuthenticated ? <Home/> : <Navigate to="/login"/>}/>
                            <Route path="/user/settings" element={<UserSettings />}/>
                            <Route path="/settings" element={<MainSettings />}/>
                            <Route path="*" element={<NotFound/>}/>
                            {menu.map(item =>
                                <Route key={item.id} path={item.link} element={item.page}/>
                            )}
                        </Route>
                    </Routes>
                </Router>
            </>
        );
    }
}

export default App;