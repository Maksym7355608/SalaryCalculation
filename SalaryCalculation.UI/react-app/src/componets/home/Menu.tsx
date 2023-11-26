import BasePageModel from "../BasePageModel";
import {EPermission} from "../../models/BaseModels";
import {NavLink} from "react-router-dom";
import React, {ReactElement} from "react";
import {OrganizationSettings} from "../organization/OrganizationSettings";

interface MenuItem{
    id: number;
    page: ReactElement;
    text: string;
    icon: string;
    link: string;
    ref: string;
    parentId: number | undefined;
}

export default class Menu extends BasePageModel {

    private getItem(id : number, page: ReactElement, text : string, icon : string, link : string, ref: string, parentId : number | undefined = undefined) : MenuItem  {
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
    getMenuItemsWithLinks() : MenuItem[] {
        const permissions = this.user?.permissions ?? [];
        let items : MenuItem[] = [];
        permissions.map((permission) => {
            let item : MenuItem | undefined;
            switch (permission) {
                case EPermission.organizationSettings :
                    item = this.getItem(permission, <OrganizationSettings id={this.user.organization}/>, "Налаштування \nорганізації", "build_circle", `/organization/${this.user.organization}/settings`, `/organization/:id/settings`);
                    break;
                case EPermission.searchEmployees :
                    item = this.getItem(permission, <OrganizationSettings id={this.user.organization}/>,"Пошук працівників", "group", `/`, '/');
                    break;
                case EPermission.searchSchedules :
                    item = this.getItem(permission, <OrganizationSettings id={this.user.organization}/>,"Табелювання", "calendar_today", `/schedule/search`, `/schedule/search`);
                    break;
                case EPermission.viewCalculation :
                    item = this.getItem(permission, <OrganizationSettings id={this.user.organization}/>,"Розрахунок", "calculate", `/calculation/search`, `/calculation/search`);
                    break;
                case EPermission.viewDictionary :
                    item = this.getItem(permission, <OrganizationSettings id={this.user.organization}/>,"Довідник", "feed", `/dictionary`, `/dictionary`);
                    break;
                case EPermission.createDocuments :
                    item = this.getItem(permission, <OrganizationSettings id={this.user.organization}/>,"Звітність", "insert_chart", `/reports`, `/reports`);
                    break;
                default:
                    item = undefined;
            }
            if(item)
                items.push(item);
        });
        return items;
    }
    render() {
        let items = this.getMenuItemsWithLinks();
        return (
            <div>
                <ul className="position-relative">
                    <li>
                        <NavLink to="/user/settings" className="menu-item">
                            <i className="material-icons">account_circle</i>  {`${this.user.firstName} ${this.user.lastName}`}
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
}