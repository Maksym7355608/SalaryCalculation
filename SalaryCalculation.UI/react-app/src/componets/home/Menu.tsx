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
    parentId: number | undefined;
}

export default class Menu extends BasePageModel {

    private getItem(id : number, page: ReactElement, text : string, icon : string, link : string, parentId : number | undefined = undefined) : MenuItem  {
        return {
            id: id,
            page: page,
            text: text,
            icon: icon,
            link: link,
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
                    item = this.getItem(permission, <OrganizationSettings organization={this.user.organization}/>, "Налаштування організації", "build_circle", `/organization/${this.user.organization}/settings`);
                    break;
                case EPermission.searchEmployees :
                    item = this.getItem(permission, <OrganizationSettings organization={this.user.organization}/>,"Пошук працівників", "group", `/`);
                    break;
                case EPermission.searchSchedules :
                    item = this.getItem(permission, <OrganizationSettings organization={this.user.organization}/>,"Табелювання", "calendar_today", `/schedule/search`);
                    break;
                case EPermission.viewCalculation :
                    item = this.getItem(permission, <OrganizationSettings organization={this.user.organization}/>,"Розрахунок", "calculate", `/calculation/search`);
                    break;
                case EPermission.viewDictionary :
                    item = this.getItem(permission, <OrganizationSettings organization={this.user.organization}/>,"Довідник", "feed", `/dictionary`);
                    break;
                case EPermission.createDocuments :
                    item = this.getItem(permission, <OrganizationSettings organization={this.user.organization}/>,"Звітність", "insert_chart", `/reports`);
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
                <ul>
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