import {OrganizationDto, OrganizationUnitDto} from "../../models/DTO";
import {Col, Container, Row} from "react-bootstrap";
import "primereact/resources/themes/lara-light-indigo/theme.css";
import {useEffect, useState} from "react";
import {RestUnitOfWork} from "../../store/rest/RestUnitOfWork";
import {user} from "../../store/actions";
import UnitSettings from "../../componets/organization/UnitSettings";
import PositionSettings from "../../componets/organization/PositionSettings";


export default function OrganizationSettings() {
    const [organization, setOrganization] = useState({} as OrganizationDto);
    const [organizationUnits, setUnits] = useState<OrganizationUnitDto[]>([]);
    const restClient = new RestUnitOfWork();

    useEffect(() => {
        restClient.organization.getOrganizationAsync(user.organization)
            .then(result => {
                setOrganization(result);
            });
        restClient.organization.getOrganizationUnitsAsync(user.organization)
            .then(result => {
                setUnits(result);
            })
    });

    return (
        <Container fluid>
            <Row className="d-flex justify-content-center">
                <h4 className="text-center">Організація</h4>
            </Row>
            <Row>
                <Col><button className="btn btn-toolbar">Налаштування прав доступу</button></Col>
                <Col><button className="btn btn-toolbar">Редагування організації</button></Col>
            </Row>
            <Row>
                <Col><button className="btn btn-toolbar">Перегляд організацій</button></Col>
                <Col><button className="btn btn-toolbar">Створення організації</button></Col>
            </Row>
            <UnitSettings units={organizationUnits}/>
            <PositionSettings  units={organizationUnits}/>
        </Container>
    );
}