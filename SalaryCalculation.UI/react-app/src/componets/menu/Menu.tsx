import {EPermission, UserModel} from "../../models/BaseModels";
import React, {ReactElement} from "react";
import {OrganizationSettings} from "../organization/OrganizationSettings";
import {NavLink} from "react-router-dom";

interface MenuItem{
    id: number;
    page: ReactElement;
    text: string;
    icon: string;
    link: string;
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
                {items.map(item =>
                    <li key={item.id}>
                        <NavLink to={item.link} className="menu-item">
                            <i className="material-icons">{item.icon}</i>  {item.text}
                        </NavLink>
                    </li>
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

    const getItem = (id : number, page: ReactElement, text : string, icon : string, link : string, ref: string, parentId : number | undefined = undefined) : MenuItem =>  {
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
        let item : MenuItem | undefined;
        switch (permission) {
            case EPermission.organizationSettings :
                item = getItem(permission, <OrganizationSettings id={user.organization}/>, "Налаштування \nорганізації", "build_circle", `/organization/${user.organization}/settings`, `/organization/:id/settings`);
                break;
            case EPermission.searchEmployees :
                item = getItem(permission, <OrganizationSettings id={user.organization}/>,"Пошук працівників", "group", `/`, '/');
                break;
            case EPermission.searchSchedules :
                item = getItem(permission, <OrganizationSettings id={user.organization}/>,"Табелювання", "calendar_today", `/schedule/search`, `/schedule/search`);
                break;
            case EPermission.viewCalculation :
                item = getItem(permission, <OrganizationSettings id={user.organization}/>,"Розрахунок", "calculate", `/calculation/search`, `/calculation/search`);
                break;
            case EPermission.viewDictionary :
                item = getItem(permission, <OrganizationSettings id={user.organization}/>,"Довідник", "feed", `/dictionary`, `/dictionary`);
                break;
            case EPermission.createDocuments :
                item = getItem(permission, <OrganizationSettings id={user.organization}/>,"Звітність", "insert_chart", `/reports`, `/reports`);
                break;
            default:
                item = undefined;
        }
        if(item)
            items.push(item);
    });
    return items;
}