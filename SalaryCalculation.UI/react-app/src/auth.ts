import {jwtDecode} from "jwt-decode";
import browserHistory from "./browserHistory";

export const authenticateAsync = async(token : string) => {
    const decoded = jwtDecode(token);
    const expired = Date.now() >= (decoded.exp ?? 0) * 1000;

    if (expired) {
        await logout();
    }
};

export const login = async(token : string) => {
    localStorage.setItem("token", token);
    browserHistory.push('/');
};

export const logout = () => {
    localStorage.clear()
    browserHistory.push("/login");
};