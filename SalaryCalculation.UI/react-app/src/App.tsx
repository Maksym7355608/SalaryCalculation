import React, {Component} from 'react';
import { BrowserRouter as Router, Route, Routes, Navigate } from "react-router-dom";
import './styles/App.css';

import Login from "./componets/identity/Login";
import Signup from "./componets/identity/Singup";
import Home from "./componets/home/Home";

const isUserAuthenticated = !!localStorage.getItem('token');

class App extends Component {
    render() {
        return (
            <Router>
                <div></div>
                <Routes>
                    <Route path="/" element={isUserAuthenticated ? <Home token={localStorage.getItem('token') as string}/> : <Navigate to="/login" />} />
                    <Route path="/login" element={isUserAuthenticated ? <Navigate to="/" /> : <Login />} />
                    <Route path="/signup" element={<Signup />} />
                </Routes>
            </Router>
        );
    }
}

export default App;