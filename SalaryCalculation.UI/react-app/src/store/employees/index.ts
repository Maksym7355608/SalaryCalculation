import {IHomeForm} from "../../models/employees/forms";
import {
    EmployeeCreateCommand,
    EmployeeSearchCommand,
    EmployeeUpdateCommand
} from "../../models/commands/OrganizationCommands";
import { user } from "../actions";
import RestUnitOfWork from "../rest/RestUnitOfWork";
import {EmployeeDto, PositionDto} from "../../models/DTO";
import {EmployeeShortModel} from "../../models/ShortModels";
import {EmployeeModel} from "../../models/employees";

export const searchEmployees = async (data: IHomeForm) => {
    const command: EmployeeSearchCommand = {
        organizationId: user().organization,
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

export const mapPositionsToIdNamePair = (positions?: PositionDto[], selectedUnit?: number) => {
    positions = positions ?? [];
    if(selectedUnit)
        positions = positions.filter(p => p.organizationUnitId === selectedUnit);
    return positions.map(p => {
        return {
            id: p.id,
            name: p.name
        }
    });
}

export const mapEmployeeModelToUpdateCommand = (data: EmployeeModel) => {
    return {
        id: data.id,
        organizationUnitId: data.organizationId,
        positionId: data.positionId,
        regimeId: data.regimeId,
        firstName: data.firstName,
        middleName: data.middleName,
        lastName: data.lastName,
        shortName: data.shortName,
        nameGenitive: data.nameGenitive,
        dateSalaryUpdated: data.dateFromSalary,
        salary: data.salary,
        marriedStatus: data.marriedStatus,
        benefits: data.benefits,
        bankAccount: data.bankAccount,
        dateTo: data.dateTo,
        phone: data.phone,
        email: data.email,
        telegram: data.telegram,
    } as EmployeeUpdateCommand;
}

export const mapEmployeeModelToCreateCommand = (data: EmployeeModel) => {
    return {
        organizationId: data.organizationId,
        organizationUnitId: data.organizationId,
        positionId: data.positionId,
        regimeId: data.regimeId,
        firstName: data.firstName,
        middleName: data.middleName,
        lastName: data.lastName,
        shortName: data.shortName,
        nameGenitive: data.nameGenitive,
        dateFrom: data.dateFrom,
        salary: data.salary,
        sex: data.sex,
        marriedStatus: data.marriedStatus,
        benefits: data.benefits,
        bankAccount: data.bankAccount,
        dateTo: data.dateTo,
        phone: data.phone,
        email: data.email,
        telegram: data.telegram,
    } as EmployeeCreateCommand;
}