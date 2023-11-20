import BasePageModel from "../BasePageModel";
import {EPermission} from "../../models/BaseModels";
import {Link} from "react-router-dom";
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
                    item = this.getItem(permission, <OrganizationSettings organization={this.user.organization}/>, "Налаштування організації", "fa-wrench", `/organization/${this.user.organization}/settings`);
                    break;
                case EPermission.searchEmployees :
                    item = this.getItem(permission, <OrganizationSettings organization={this.user.organization}/>,"Пошук працівників", "fa-user-group", `/employees/search`);
                    break;
                case EPermission.searchSchedules :
                    item = this.getItem(permission, <OrganizationSettings organization={this.user.organization}/>,"Табелювання", "fa-calendar", `/schedule/search`);
                    break;
                case EPermission.viewCalculation :
                    item = this.getItem(permission, <OrganizationSettings organization={this.user.organization}/>,"Розрахунок", "fa-calculator", `/calculation/search`);
                    break;
                case EPermission.viewDictionary :
                    item = this.getItem(permission, <OrganizationSettings organization={this.user.organization}/>,"Довідник", "fa-book", `/dictionary`);
                    break;
                case EPermission.createDocuments :
                    item = this.getItem(permission, <OrganizationSettings organization={this.user.organization}/>,"Звітність", "fa-chart-simple", `/reports`);
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
            <div className="dropdown-menu">
                <ul>
                    {items.map(item =>
                        <li key={item.id}>
                            <Link to={item.link} className="menu-item">
                                <i className={`fa ${item.icon}`}/> {item.text}
                            </Link>
                        </li>
                    )}
                </ul>
            </div>
        );
    }
}