import Header from "./Header";
import Footer from "./Footer";
import Menu from "./Menu";
import React from "react";
import {Outlet} from "react-router-dom";
import { Container, Row, Col } from 'react-bootstrap';

const Layout = () => {
    return (
        <div className="vh-100">
            <Container fluid className="inbox">
                <Row>
                    <Col md={2} className="menu">
                        <Menu/>
                    </Col>
                    <Col md={10}>
                        <main className="main">
                            <Header/>
                            <div className="inbox">
                                <Outlet/>
                                <span id="requestInvalid" className="text-danger"></span>
                                <span id="responseInvalid" className="text-danger"></span>
                            </div>
                        </main>
                        <footer>
                            <Footer/>
                        </footer>
                    </Col>
                </Row>
            </Container>
        </div>
    );
}

export default Layout;