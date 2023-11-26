import Header from "./Header";
import Footer from "./Footer";
import Menu from "./Menu";
import React from "react";
import {Outlet} from "react-router-dom";
import {Col, Container, Row} from 'react-bootstrap';

const Layout = () => {
    return (
        <div className="vh-100 w-100 h-100 d-flex">
            <Container fluid className="inbox">
                <Row>
                    <Col md={2} className="menu">
                        <Menu/>
                    </Col>
                    <Col md={10} className="main">
                        <main>
                            <Header/>
                            <div className="inbox">
                                <Outlet/>
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