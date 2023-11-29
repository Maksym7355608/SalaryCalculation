import React from 'react';
import { Provider } from "react-redux";
import {BrowserRouter as Router, Route, Routes} from "react-router-dom";
import '../styles/App.css';
import '../styles/Layout.css';
import {PrimeReactProvider} from 'primereact/api';

import store from "../store";
import browserHistory from "../browserHistory";
import Login from "../componets/identity/Login";
import Signup from "../componets/identity/Singup";
import {NotFound} from "../componets/notFound/NotFound";
import Layout from "../componets/layout/Layout";
import {AuthLayout} from "../componets/layout/AuthLayout";

const App = () => {
    const isUserAuthenticated = !!localStorage.getItem('token');
    return (
        <Provider store={store}>
            <PrimeReactProvider>
                <Router>
                    <Routes>
                        <Route path="*" element={isUserAuthenticated ? <NotFound/> :
                            <Layout title="Not Found">
                                <NotFound/>
                            </Layout>}/>
                        <Route path="/login" element={<AuthLayout title="Вхід">
                            <Login/>
                        </AuthLayout>}/>
                        <Route path="/signup" element={<AuthLayout title="Реєстрація">
                            <Signup/>
                        </AuthLayout>}/>
                        {/*TODO: make list private routes*/}
                    </Routes>
                </Router>
            </PrimeReactProvider>
        </Provider>
    );
}

/*class App1 extends Component {
    render() {
        let menu = new Menu({}).getMenuItemsWithLinks();
        return (
            <PrimeReactProvider>
                <Router navigator={browserHistory} location={isUserAuthenticated ? "/" : "/login"}>
                    <Routes>
                        <Route path="/login" element={isUserAuthenticated ? <Navigate to="/"/> : <Login/>}/>
                        <Route path="/signup" element={<Signup/>}/>
                        <Route path="/" element={<Layout/>}>
                            <Route key={0} index element={isUserAuthenticated ? <Home/> : <Navigate to="/login"/>}/>
                            <Route path="/user/settings" element={<UserSettings />}/>
                            <Route path="/settings" element={<MainSettings />}/>
                            <Route path="*" element={<NotFound/>}/>
                            {menu.map(item =>
                                <Route key={item.id} path={item.ref} element={item.page}/>
                            )}
                        </Route>
                    </Routes>
                </Router>
            </PrimeReactProvider>
        );
    }
}*/

export default App;