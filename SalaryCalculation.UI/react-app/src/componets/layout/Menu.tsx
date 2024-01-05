import {UserModel} from "../../models/BaseModels";
import React, {ReactElement, useState} from "react";
import OrganizationSettings from "../../views/organization/OrganizationSettings";
import {NavLink} from "react-router-dom";
import Home from "../../views/home/Home";
import OrganizationPermissions from "../../views/organization/OrganizationPermissions";
import {Organization} from "../../views/organization/Organization";
import Employee from "../../views/employees/Employee";
import {EPermission} from "../../models/Enums";
import ScheduleSearch from "../../views/schedule/Search";
import Schedule from "../../views/schedule/Schedule";
import { Nav } from "react-bootstrap";
import {Regime} from "../../views/schedule/Regime";
import CalculationSearch from "../../views/calculation/Search";

interface MenuItem{
    id: number | string;
    page: ReactElement;
    text: string;
    icon?: string;
    link?: string;
    ref: string;
    parentId: number | undefined;
}

const user = JSON.parse(localStorage.getItem('user') as string) as UserModel;

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
                <Nav hidden={condition} className='bg-blue-900'>
                    <Nav.Item className='mt-1'>
                        <NavLink to={item.link} className="menu-item-child">
                            <i className="material-icons small">{item.icon}</i> {item.text}
                        </NavLink>
                    </Nav.Item>
                        {
                            children.map(ch => ch.link && (
                                <Nav.Item className='mt-1'>
                                    <NavLink to={ch.link} className="menu-item-child">
                                        <i className="material-icons small">{ch.icon}</i> {ch.text}
                                    </NavLink>
                                </Nav.Item>
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
                        <i className="material-icons">account_circle</i> {`${user.firstName} ${user.lastName}`}
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

    const permissions = user?.permissions ?? [];
    let items : MenuItem[] = [];

    const getItem = (id: number | string, page: ReactElement, text: string, ref: string, icon?: string, link?: string, parentId?: number) : MenuItem =>  {
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
                    getItem(permission, <OrganizationSettings/>, "Налаштування організації",`/organization/:id/settings`, "build_circle", `/organization/${user.organization}/settings`),
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
                item = [getItem(permission, <CalculationSearch/>,"Розрахунок", `/calculation/search`, "calculate", `/calculation/search`)];
                break;
            case EPermission.viewDictionary :
                item = [getItem(permission, <OrganizationSettings/>,"Довідник", `/dictionary`, "feed", `/dictionary`)];
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