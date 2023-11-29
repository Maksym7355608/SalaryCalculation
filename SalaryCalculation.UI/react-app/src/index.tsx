import React from 'react';
import ReactDOM from 'react-dom/client';
import './styles/index.css';
import App from './views/App';
import {authenticateAsync} from "./auth";

(async() => {
    const token = localStorage.getItem("token");

    if (token) {
        await authenticateAsync(token);
    }

    const container = document.getElementById("root");
    const root = ReactDOM.createRoot(container as HTMLElement);
    root.render(
        <React.StrictMode>
            <App />
        </React.StrictMode>
    );
})();


