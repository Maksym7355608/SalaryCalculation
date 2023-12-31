import React, {ReactElement, useState} from "react";
import {NavLink} from "react-router-dom";
import {EPermission} from "../../models/Enums";
import { Nav, Row } from "react-bootstrap";
import OrganizationSettings from "../../views/organization/OrganizationSettings";
import Home from "../../views/home/Home";
import OrganizationPermissions from "../../views/organization/OrganizationPermissions";
import Organization from "../../views/organization/Organization";
import Employee from "../../views/employees/Employee";
import ScheduleSearch from "../../views/schedule/Search";
import Schedule from "../../views/schedule/Schedule";
import Regime from "../../views/schedule/Regime";
import CalculationSearch from "../../views/calculation/Search";
import CalculationDetail from "../../views/calculation/Details";
import OperationsData from "../../views/dictionary/Operations";
import BaseAmounts from "../../views/dictionary/BaseAmounts";
import Formulas from "../../views/dictionary/Formulas";
import IndexFormula from "../../views/dictionary/IndexFormula";
import {Regimes} from "../../views/dictionary/Regimes";
import {user} from "../../store/actions";

interface MenuItem{
    id: number | string;
    page: ReactElement;
    text: string;
    icon?: string;
    link?: string;
    ref: string;
    parentId?: number | string;
}

export function Menu() {
    const items = InitMenu();
    const [open, setOpen] = useState<number | string | undefined>();

    const renderMenuItem = (item: MenuItem) => {
        if(!item.link || item.parentId)
            return null;
        if(!items.some(i => i.parentId == item.id))
            return (
                <Nav.Item key={item.id}  className='mt-3'>
                    <NavLink to={item.link} className="menu-item">
                        <i className="material-icons">{item.icon}</i> {item.text}
                    </NavLink>
                </Nav.Item>);

        const children = items.filter(i => i.parentId == item.id);
        const condition = !open || open != item.id;
        return (
            <Nav.Item key={item.id} className='mt-3'>
                <label onClick={() => condition ? setOpen(item.id) : setOpen(undefined)} className="menu-item">
                    <i className="material-icons">{item.icon}</i> {item.text} <span className="material-icons">{condition ? "expand_more" : "expand_less"}</span>
                </label>
                <Nav variant="pills" hidden={condition} className='flex-column bg-blue-900'>
                    <Row>
                        <Nav.Item className='mt-1'>
                            <NavLink to={item.link} className="menu-item-child">
                                <i className="material-icons small">{item.icon}</i> {item.text}
                            </NavLink>
                        </Nav.Item>
                    </Row>
                    {
                        children.map(ch => ch.link && (
                            <Row>
                                <Nav.Item className='mt-1'>
                                    <NavLink to={ch.link} className="menu-item-child">
                                        <i className="material-icons small">{ch.icon}</i> {ch.text}
                                    </NavLink>
                                </Nav.Item>
                            </Row>
                        ))
                    }

                </Nav>
            </Nav.Item>
        );
    }

    return (
        <div>
            <Nav variant="pills" className="flex-column mt-2">
                <Nav.Item className='mt-2'>
                    <NavLink to="/user/settings" className="menu-item">
                        <i className="material-icons">account_circle</i> {`${user().firstName} ${user().lastName}`}
                    </NavLink>
                </Nav.Item>
                {items.map(item => {
                    return renderMenuItem(item);
                })}
                <Nav.Item className="settings-link">
                    <NavLink to="/settings" className="menu-item">
                        <i className="material-icons">settings</i> Налаштування
                    </NavLink>
                </Nav.Item>
            </Nav>
        </div>
    );
}

export function InitMenu() : MenuItem[] {
    const permissions = user()?.permissions ?? [];
    let items : MenuItem[] = [];

    const getItem = (id: number | string, page: ReactElement, text: string, ref: string, icon?: string, link?: string, parentId?: number | string) : MenuItem =>  {
        return {
            id: id,
            page: page,
            text: text,
            icon: icon,
            link: link,
            ref: ref,
            parentId: parentId
        };
    }

    permissions.map((permission) => {
        let item : MenuItem[] | undefined;
        switch (permission) {
            case EPermission.organizationSettings :
                item = [
                    getItem(permission, <OrganizationSettings/>, "Налаштування організації",`/organization/:id/settings`, "build_circle", `/organization/${user().organization}/settings`),
                    getItem('permissions', <OrganizationPermissions/>, "Налаштування прав доступу", `/organization/:id/permissions`),
                    getItem('organization', <Organization/>, "Організація", `/organization/:id`),
                ]
                break;
            case EPermission.searchEmployees :
                item = [
                    getItem(permission, <Home/>,"Пошук працівників", `/`, "group", '/'),
                ];
                break;
            case EPermission.createEmployees:
                item = [getItem(permission, <Employee/>, "Управління працівниками", `/employee/:id`, '', '/employee/create', 3)];
                break;
            case EPermission.searchSchedules :
                item = [
                    getItem(permission, <ScheduleSearch/>,"Табелювання", `/schedule/search`, "calendar_today", `/schedule/search`),
                    getItem('regime', <Regime/>, "Режим роботи", `/schedule/regime/:id`, 'schedule', '/schedule/regime/create', permission),
                    getItem('schedule', <Schedule/>,"Табелювання працівника", `/schedule/:id/:period`)
                ];
                break;
            case EPermission.viewCalculation :
                item = [
                    getItem(permission, <CalculationSearch/>,"Розрахунок", `/calculation/search`, "calculate", `/calculation/search`),
                    getItem('calc-details', <CalculationDetail/>, "Деталізація розрахунку", '/calculation/details/:id')
                ];
                break;
            case EPermission.viewDictionary :
                item = [
                    getItem('operationData', <OperationsData/>,"Довідник", `/dictionary/operations`, "feed", `/dictionary/operations`),
                    getItem('baseAmounts', <BaseAmounts/>, "Базові суми", `/dictionary/base-amounts`, 'foundation', `/dictionary/base-amounts`, 'operationData'),
                    getItem('formulas', <Formulas/>, "Формули", `/dictionary/formulas`, 'functions', `/dictionary/formulas`, 'operationData'),
                    getItem('regimes', <Regimes/>, "Режими", `/dictionary/regimes`, 'schedule', `/dictionary/regimes`, 'operationData'),
                    getItem('formula', <IndexFormula/>, "Управління Формулами", `/dictionary/formula/:id`),
                ];
                break;
            case EPermission.createDocuments :
                item = [getItem(permission, <OrganizationSettings/>,"Звітність", `/reports`, "insert_chart", `/reports`)];
                break;
            default:
                item = undefined;
        }
        if(item)
            items = [...items, ...item];
    });
    return items;
}