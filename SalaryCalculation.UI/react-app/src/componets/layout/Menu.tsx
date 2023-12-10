import {UserModel} from "../../models/BaseModels";
import React, {ReactElement} from "react";
import OrganizationSettings from "../../views/organization/OrganizationSettings";
import {NavLink} from "react-router-dom";
import Home from "../../views/home/Home";
import OrganizationPermissions from "../../views/organization/OrganizationPermissions";
import {Organization} from "../../views/organization/Organization";
import Employee from "../../views/employees/Employee";
import {EPermission} from "../../models/Enums";

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
    return (
        <div>
            <ul className="position-relative">
                <li>
                    <NavLink to="/user/settings" className="menu-item">
                        <i className="material-icons">account_circle</i>  {`${user.firstName} ${user.lastName}`}
                    </NavLink>
                </li>
                {items.map(item => {
                        return item.link && <li key={item.id}>
                            <NavLink to={item.link} className="menu-item">
                                <i className="material-icons">{item.icon}</i> {item.text}
                            </NavLink>
                        </li>
                    }
                )}
                <li className="settings-link">
                    <NavLink to="/settings" className="menu-item">
                        <i className="material-icons">settings</i> Налаштування
                    </NavLink>
                </li>
            </ul>
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
                item = [getItem(permission, <Home/>,"Пошук працівників", `/`, "group", '/')];
                break;
            case EPermission.createEmployees:
                item = [getItem(permission, <Employee/>, "Управління працівниками", `/employees/:id`)];
                break;
            case EPermission.searchSchedules :
                item = [getItem(permission, <OrganizationSettings/>,"Табелювання", `/schedule/search`, "calendar_today", `/schedule/search`)];
                break;
            case EPermission.viewCalculation :
                item = [getItem(permission, <OrganizationSettings/>,"Розрахунок", `/calculation/search`, "calculate", `/calculation/search`)];
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