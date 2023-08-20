using AutoMapper;
using Microsoft.AspNetCore.Mvc;
using Organization.App.Abstract;
using Organization.App.Commands;
using SalaryCalculation.Shared.Common.Attributes;
using Serilog.Events;
using SerilogTimings;

namespace Organization.Api.Controllers;

[ApiController]
[HandleException]
[Route("api/[controller]")]
public class OrganizationsController : BaseOrganizationController
{
    public OrganizationsController(IMapper mapper, IOrganizationCommandHandler organizationCommandHandler,
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
        return Ok(new AjaxResponse { IsSuccess = ModelState.IsValid, Errors = Errors, Data = organization });
    }

    [HttpGet("all")]
    public async Task<IActionResult> GetOrganizationsAsync()
    {
        var organizations = await OrganizationCommandHandler.GetOrganizationsAsync();
        return Ok(new AjaxResponse { IsSuccess = true, Data = organizations });
    }

    [HttpPost("create")]
    public async Task<IActionResult> CreateOrganizationAsync([FromBody] OrganizationCreateCommand command)
    {
        using var op = Operation.At(LogEventLevel.Debug).Begin("Organization {0} creating started", command.Name);
        await OrganizationCommandHandler.CreateOrganizationAsync(command);
        op.Complete();
        return Ok(new AjaxResponse { IsSuccess = true, Errors = Errors});
    }

    [HttpPut("update/{id}")]
    public async Task<IActionResult> UpdateOrganizationAsync([FromRoute] int id,
        [FromBody] OrganizationUpdateCommand command)
    {
        using var op = Operation.At(LogEventLevel.Debug).Begin("Organization {0} updating started", command.Name);
        command.Id = id;
        var updated = await OrganizationCommandHandler.UpdateOrganizationAsync(command);

        op.Complete();
        if (updated)
            return Ok(new AjaxResponse { IsSuccess = true,  });
        else
            return BadRequest(new AjaxResponse { IsSuccess = false, Errors = Errors });
    }

    [HttpDelete("delete/{id}")]
    public async Task<IActionResult> DeleteOrganizationAsync([FromRoute] int id)
    {
        using var op = Operation.At(LogEventLevel.Debug).Begin("Organization with id {0} deleting started", id);
        var deleted = await OrganizationCommandHandler.DeleteOrganizationAsync(id);
        op.Complete();
        if (deleted)
            return Ok(new AjaxResponse { IsSuccess = true });
        else
            return BadRequest(new AjaxResponse { IsSuccess = false, Errors = Errors });
    }

    #region Organization Units
    
    [HttpGet("{organizationId}/units/{id}")]
    public async Task<IActionResult> GetOrganizationUnitAsync([FromRoute] int organizationId,
        [FromRoute] int id)
    {
        var unit = await OrganizationCommandHandler.GetOrganizationUnitAsync(organizationId, id);
        return Ok(new AjaxResponse { IsSuccess = true, Data = unit });
    }

    [HttpGet("{organizationId}/units/search")]
    public async Task<IActionResult> SearchOrganizationUnitsAsync([FromRoute] int organizationId,
        [FromQuery] OrganizationUnitSearchCommand command)
    {
        using var op = Operation.At(LogEventLevel.Debug).Begin("Organization units searching started");
        var units = await OrganizationCommandHandler.SearchOrganizationUnitsAsync(command);
        op.Complete();
        return Ok(new AjaxResponse { IsSuccess = true, Data = units });
    }
    
    [HttpPost("units/create")]
    public async Task<IActionResult> CreateOrganizationUnitAsync([FromBody] OrganizationUnitCreateCommand command)
    {
        using var op = Operation.At(LogEventLevel.Debug).Begin("Organization unit {0} creating started", command.Name);
        await OrganizationCommandHandler.CreateOrganizationUnitAsync(command);
        op.Complete();
        return Ok(new AjaxResponse { IsSuccess = true, Errors = Errors});
    }

    [HttpPut("units/update/{id}")]
    public async Task<IActionResult> UpdateOrganizationUnitAsync([FromRoute] int id,
        [FromBody] OrganizationUnitUpdateCommand command)
    {
        using var op = Operation.At(LogEventLevel.Debug).Begin("Organization unit {0} updating started", command.Name);
        command.Id = id;
        var updated = await OrganizationCommandHandler.UpdateOrganizationUnitAsync(command);

        op.Complete();
        if (updated)
            return Ok(new AjaxResponse { IsSuccess = true,  });
        else
            return BadRequest(new AjaxResponse { IsSuccess = false, Errors = Errors });
    }

    [HttpDelete("{organizationId}/units/delete/{id}")]
    public async Task<IActionResult> DeleteOrganizationUnitAsync([FromRoute] int organizationId, [FromRoute] int id)
    {
        using var op = Operation.At(LogEventLevel.Debug).Begin("Organization with id {0} deleting started", id);
        var deleted = await OrganizationCommandHandler.DeleteOrganizationUnitAsync(organizationId, id);
        op.Complete();
        if (deleted)
            return Ok(new AjaxResponse { IsSuccess = true });
        else
            return BadRequest(new AjaxResponse { IsSuccess = false, Errors = Errors });
    }

    #endregion

    #region Positions
    
    [HttpGet("{organizationId}/units/{organizationUnitId}/positions/{id}")]
    public async Task<IActionResult> GetPositionAsync([FromRoute] int organizationId, [FromRoute] int organizationUnitId,
        [FromRoute] int id)
    {
        var unit = await OrganizationCommandHandler.GetPositionAsync(organizationId, organizationUnitId, id);
        return Ok(new AjaxResponse { IsSuccess = true, Data = unit });
    }

    [HttpGet("{organizationId}/positions/search")]
    public async Task<IActionResult> SearchPositionAsync([FromRoute] int organizationId,
        [FromQuery] PositionSearchCommand command)
    {
        using var op = Operation.At(LogEventLevel.Debug).Begin("Organization units searching started");
        var units = await OrganizationCommandHandler.SearchPositionsAsync(command);
        op.Complete();
        return Ok(new AjaxResponse { IsSuccess = true, Data = units });
    }
    
    [HttpPost("positions/create")]
    public async Task<IActionResult> CreatePositionAsync([FromBody] PositionCreateCommand command)
    {
        using var op = Operation.At(LogEventLevel.Debug).Begin("Organization unit {0} creating started", command.Name);
        await OrganizationCommandHandler.CreatePositionAsync(command);
        op.Complete();
        return Ok(new AjaxResponse { IsSuccess = true, Errors = Errors});
    }

    [HttpPut("positions/update/{id}")]
    public async Task<IActionResult> UpdatePositionAsync([FromRoute] int id,
        [FromBody] PositionUpdateCommand command)
    {
        using var op = Operation.At(LogEventLevel.Debug).Begin("Organization unit {0} updating started", command.Name);
        command.Id = id;
        var updated = await OrganizationCommandHandler.UpdatePositionAsync(command);

        op.Complete();
        if (updated)
            return Ok(new AjaxResponse { IsSuccess = true,  });
        else
            return BadRequest(new AjaxResponse { IsSuccess = false, Errors = Errors });
    }

    [HttpDelete("{organizationId}/units/{organizationUnitId}/positions/delete/{id}")]
    public async Task<IActionResult> DeletePositionAsync([FromRoute] int organizationId, [FromRoute] int organizationUnitId, [FromRoute] int id)
    {
        using var op = Operation.At(LogEventLevel.Debug).Begin("Organization with id {0} deleting started", id);
        var deleted = await OrganizationCommandHandler.DeletePositionAsync(organizationId, organizationUnitId, id);
        op.Complete();
        if (deleted)
            return Ok(new AjaxResponse { IsSuccess = true });
        else
            return BadRequest(new AjaxResponse { IsSuccess = false, Errors = Errors });
    }

    #endregion
}

public class AjaxResponse
{
    public bool IsSuccess { get; set; }
    public string[]? Errors { get; set; }
    public object Data { get; set; }
}