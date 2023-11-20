import React, {Component} from 'react';
import {BrowserRouter as Router, Navigate, Route, Routes} from "react-router-dom";
import './styles/App.css';

import Login from "./componets/identity/Login";
import Signup from "./componets/identity/Singup";
import Home from "./componets/home/Home";
import Menu from "./componets/home/Menu";
import {NOTFOUND} from "dns";
import {NotFound} from "./componets/notFound/NotFound";

const isUserAuthenticated = !!localStorage.getItem('token');

class App extends Component {
    render() {
        let menu = new Menu({}).getMenuItemsWithLinks();
        return (
            <Router>
                <Routes>
                    <Route path="/" element={isUserAuthenticated ? <Home/> : <Navigate to="/login"/>}/>
                    <Route path="/login" element={isUserAuthenticated ? <Navigate to="/"/> : <Login/>}/>
                    <Route path="/signup" element={<Signup/>}/>
                    <Route element={<NotFound/>}/>

                    {menu.map(item =>
                        <Route path={item.link} element={item.page}/>
                    )}
                </Routes>
            </Router>
        );
    }
}

export default App;