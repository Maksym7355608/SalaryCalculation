using System.ComponentModel;
using System.ComponentModel.DataAnnotations;

namespace SalaryCalculation.Data.Enums;

public enum EPermission
{
    [Description("Role control")]
    RoleControl = 1,
    [Description("Organization settings")]
    OrganizationSettings = 2,
    [Description("Search employees")]
    SearchEmployees = 3,
    [Description("Create employees")]
    CreateEmployees = 4,
    [Description("Delete employees")]
    DeleteEmployees = 5,
    [Description("View schedule")]
    ViewSchedule = 6,
    [Description("Search schedules")]
    SearchSchedules = 7,
    [Description("Calculate schedules")]
    CalculateSchedules = 8,
    [Description("View calculation")]
    ViewCalculation = 9,
    [Description("Calculation salaries")]
    CalculationSalaries = 10,
    [Description("View dictionary")]
    ViewDictionary = 11,
    [Description("Create documents")]
    CreateDocuments = 12,
    [Description("Create organization")]
    CreateOrganization = 13,
}