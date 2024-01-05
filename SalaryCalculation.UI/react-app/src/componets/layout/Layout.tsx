import Header from "./Header";
import Footer from "./Footer";
import {Menu} from "./Menu";
import React from "react";
import {Col, Container, Row, Tab} from 'react-bootstrap';
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
        <Tab.Container id="left-tabs-example" defaultActiveKey="first">
            <Container fluid className="inbox vh-100 w-100 h-100">
                <Row>
                    <Col md={2} className="menu p-0 ps-2">
                        <Menu/>
                    </Col>
                    <Col md={10} className="main">
                        <main>
                            <Header title={title}/>
                            <div>
                                {children}
                                <span id="request-validation" className="text-danger"></span>
                                <span id="response-validation" className="text-danger"></span>
                            </div>
                        </main>
                        <footer>
                            <Footer/>
                        </footer>
                    </Col>
                </Row>
            </Container>
        </Tab.Container>
    );
}

export default Layout;