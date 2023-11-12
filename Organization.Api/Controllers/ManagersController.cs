using AutoMapper;
using Microsoft.AspNetCore.Mvc;
using Organization.App.Abstract;
using Organization.App.Commands;
using SalaryCalculation.Shared.Common.Attributes;
using Serilog.Events;
using SerilogTimings;

namespace Organization.Api.Controllers;

public class ManagersController : BaseOrganizationController
{
    public ManagersController(IMapper mapper, IOrganizationCommandHandler organizationCommandHandler, IManagerCommandHandler managerCommandHandler, IEmployeeCommandHandler employeeCommandHandler) : base(mapper, organizationCommandHandler, managerCommandHandler, employeeCommandHandler)
    {
    }

    [HttpPost("add")]
    public async Task<IActionResult> AddManagerAsync([FromQuery] ManagerAddCommand command)
    {
        using var op = Operation.At(LogEventLevel.Debug).Begin("Manager adding started");
        await ManagerCommandHandler.AddManagerToOrganizationAsync(command);
        op.Complete();
        return GetAjaxResponse(IsValid, Errors);
    }
    
    [HttpPost("remove/{id}")]
    public async Task<IActionResult> AddManagerAsync([FromRoute] int id)
    {
        using var op = Operation.At(LogEventLevel.Debug).Begin("Manager removing started");
        await ManagerCommandHandler.RemoveManagerFromOrganizationAsync(id);
        op.Complete();
        return GetAjaxResponse(IsValid, Errors);
    }
}