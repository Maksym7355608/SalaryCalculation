import {OrganizationUnitDto} from "../../models/DTO";
import {Col, Container, Row} from "react-bootstrap";
import {useEffect, useState} from "react";
import RestUnitOfWork from "../../store/rest/RestUnitOfWork";
import {user} from "../../store/actions";
import UnitSettings from "../../componets/organization/UnitSettings";
import PositionSettings from "../../componets/organization/PositionSettings";
import {Link} from "react-router-dom";


export default function OrganizationSettings() {
    const [organizationUnits, setUnits] = useState<OrganizationUnitDto[]>([]);
    const restClient = new RestUnitOfWork();

    useEffect(() => {
        restClient.organization.getOrganizationUnitsAsync(user().organization)
            .then(result => {
                setUnits(result);
            })
    }, []);

    return (
        <Container fluid>
            <Row className="d-flex justify-content-center">
                <h4 className="text-center">Організація</h4>
            </Row>
            <Row>
                <Col><Link to={`/organization/${user().organization}/permissions`} className="btn btn-toolbar">Налаштування прав доступу</Link></Col>
                <Col><Link to={`/organization/${user().organization}`} className="btn btn-toolbar">Редагування організації</Link></Col>
            </Row>
            <Row>
                <Col><button className="btn btn-toolbar">Перегляд організацій</button></Col>
                <Col><Link to={`/organization/${'new'}`} className="btn btn-toolbar">Створення організації</Link></Col>
            </Row>
            <UnitSettings units={organizationUnits}/>
            <PositionSettings  units={organizationUnits}/>
        </Container>
    );
}