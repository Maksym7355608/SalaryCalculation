import {IHomeForm} from "../../models/employees/forms";
import {EmployeeSearchCommand} from "../../models/commands/OrganizationCommands";
import { user } from "../actions";
import {RestUnitOfWork} from "../rest/RestUnitOfWork";
import {EmployeeDto} from "../../models/DTO";
import {EmployeeShortModel} from "../../models/ShortModels";

export const searchEmployees = async (data: IHomeForm) => {
    const command: EmployeeSearchCommand = {
        organizationId: user.organization,
        rollNumber: data.rollNumber,
        organizationUnitId: data.organizationUnit,
        positionId: data.position,
        dateFrom: data.date,
        dateTo: new Date(Date.now()),
        salaryFrom: data.salaryFrom,
        salaryTo: data.salaryTo,
    };
    const restClient = new RestUnitOfWork();
    return await restClient.organization.searchEmployeesAsync(command)
}

export const mapToEmployeeShortModel = (employees: EmployeeDto[]): EmployeeShortModel[] => {
    return employees.map(employee => {
        let contacts : string = "";
        employee.contacts.map(c => {
            contacts+=c.value + ", "
        });
        return {
            id: employee.id,
            rollNumber: employee.rollNumber.toString(),
            unit: employee.organizationUnitId.name,
            position: employee.position.name,
            employeeDate: employee.dateFrom.toDateString(),
            dismissDate: employee.dateTo?.toDateString() ?? "",
            salary: employee.salaries[employee.salaries.length-1].amount,
            sex: employee.sex.toString(),
            familyStatus: employee.marriedStatus.toString(),
            kids: undefined,
            contacts: contacts
        } as EmployeeShortModel;
    });
}