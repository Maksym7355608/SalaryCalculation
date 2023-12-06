import React from 'react';
import { Provider } from "react-redux";
import {BrowserRouter as Router, Route, Routes} from "react-router-dom";
import '../styles/App.css';
import '../styles/Layout.css';
import {PrimeReactProvider} from 'primereact/api';

import store from "../store";
import Login from "./identity/Login";
import Signup from "./identity/Singup";
import {NotFound} from "./notFound/NotFound";
import Layout from "../componets/layout/Layout";
import {AuthLayout} from "../componets/layout/AuthLayout";
import {InitMenu} from "../componets/layout/Menu";

const App = () => {
    const isUserAuthenticated = !!localStorage.getItem('token');
    const menuItems = InitMenu()
    return (
        <Provider store={store}>
            <PrimeReactProvider>
                <Router>
                    <Routes>
                        <Route path="*" element={!isUserAuthenticated ? <NotFound/> :
                            <Layout title="Not Found">
                                <NotFound/>
                            </Layout>}/>
                        <Route path="/login" element={<AuthLayout title="Вхід">
                            <Login/>
                        </AuthLayout>}/>
                        <Route path="/signup" element={<AuthLayout title="Реєстрація">
                            <Signup/>
                        </AuthLayout>}/>
                        {
                            menuItems.map(item =>
                                <Route key={item.id} path={item.ref} element={<Layout title={item.text}>{item.page}</Layout>}/>
                        )}
                    </Routes>
                </Router>
            </PrimeReactProvider>
        </Provider>
    );
}
export default App;