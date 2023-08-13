using AutoMapper;
using Microsoft.AspNetCore.Mvc;
using Organization.App.Abstract;
using SalaryCalculation.Shared.Common.Attributes;
using Serilog.Events;
using SerilogTimings;

namespace Organization.Api.Controllers;

[ApiController]
[HandleException]
[Route("api/[controller]")]
public class OrganizationController : BaseOrganizationController
{
    public OrganizationController(IMapper mapper, IOrganizationCommandHandler organizationCommandHandler,
        IManagerCommandHandler managerCommandHandler, IEmployeeCommandHandler employeeCommandHandler) : base(mapper,
        organizationCommandHandler, managerCommandHandler, employeeCommandHandler)
    {
    }

    [HttpGet("{id:int}")]
    public async Task<IActionResult> GetOrganizationAsync([FromRoute] int id)
    {
        using var op = Operation.At(LogEventLevel.Debug).Begin("Organization with id {0}", id);
        var organization = await OrganizationCommandHandler.GetOrganizationAsync(id);
        op.Complete();
        return Ok(new AjaxResponse { IsSuccess = ModelState.IsValid, Data = organization });
    }
}

public class AjaxResponse
{
    public bool IsSuccess { get; set; }
    public string[]? Errors { get; set; }
    public object Data { get; set; }
}