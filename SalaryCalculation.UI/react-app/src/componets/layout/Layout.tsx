import Header from "../home/Header";
import Footer from "../home/Footer";
import Menu from "../home/Menu";
import React from "react";
import {Col, Container, Row} from 'react-bootstrap';
import {useNavigate} from "react-router-dom";

export interface LayoutProps {
    title: string;
    children: any;
}

const Layout : React.FC<LayoutProps> = ({title, children}) => {
    const navigate = useNavigate();
    if(!localStorage.getItem("token"))
    {
        navigate("/login");
        return null;
    }
    return (
        <div className="vh-100 w-100 h-100 d-flex">
            <Container fluid className="inbox">
                <Row>
                    <Col md={2} className="menu">
                        <Menu/>
                    </Col>
                    <Col md={10} className="main">
                        <main>
                            <Header title={title}/>
                            <div className="inbox">
                                { children }
                                <span id="requestInvalid" className="text-danger"></span>
                                <span id="responseInvalid" className="text-danger"></span>
                            </div>
                        </main>
                        <footer className="d-flex justify-content-center align-bottom">
                            <Footer/>
                        </footer>
                    </Col>
                </Row>
            </Container>
        </div>
    );
}

export default Layout;