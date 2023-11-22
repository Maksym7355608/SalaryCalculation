using AutoMapper;
using Microsoft.AspNetCore.Mvc;
using Organization.App.Abstract;
using Organization.App.Commands;
using SalaryCalculation.Shared.Common.Attributes;
using Serilog.Events;
using SerilogTimings;

namespace Organization.Api.Controllers;

public class EmployeesController : BaseOrganizationController
{
    public EmployeesController(IMapper mapper, IOrganizationCommandHandler organizationCommandHandler, IManagerCommandHandler managerCommandHandler, IEmployeeCommandHandler employeeCommandHandler) : base(mapper, organizationCommandHandler, managerCommandHandler, employeeCommandHandler)
    {
    }

    [HttpGet("{id:int}")]
    public async Task<IActionResult> GetEmployeeAsync([FromRoute] int id)
    {
        using var op = Operation.At(LogEventLevel.Debug).Begin("Employee with id {0}", id);
        var employee = await EmployeeCommandHandler.GetEmployeeAsync(id);
        op.Complete();
        return GetAjaxResponse(IsValid, employee, Errors);
    }

    [HttpGet("organization/{organizationId:int}")]
    public async Task<IActionResult> GetAllEmployeesAsync([FromRoute] int organizationId)
    {
        var employees = await EmployeeCommandHandler.GetAllEmployeesAsync(organizationId);
        return GetAjaxResponse(IsValid, employees, Errors);
    }

    [HttpPost("search")]
    public async Task<IActionResult> SearchEmployeesAsync([FromBody] EmployeeSearchCommand command)
    {
        var employees = await EmployeeCommandHandler.SearchEmployeesAsync(command);
        return GetAjaxResponse(IsValid, employees, Errors);
    }

    [HttpPost("create")]
    public async Task<IActionResult> CreateEmployeeAsync([FromBody] EmployeeCreateCommand command)
    {
        await EmployeeCommandHandler.CreateEmployeeAsync(command);
        return GetAjaxResponse(IsValid, Errors);
    }

    [HttpPut("update/{id:int}")]
    public async Task<IActionResult> UpdateEmployeeAsync([FromRoute] int id, [FromBody] EmployeeUpdateCommand command)
    {
        command.Id = id;
        var updated = await EmployeeCommandHandler.UpdateEmployeeAsync(command);

        return GetAjaxResponse(IsValid && updated, Errors);
    }

    [HttpDelete("delete/{id:int}")]
    public async Task<IActionResult> DeleteEmployeeAsync([FromRoute] int id)
    {
        var deleted = await EmployeeCommandHandler.DeleteEmployeeAsync(id);

        return GetAjaxResponse(IsValid && deleted, Errors);
    }
}