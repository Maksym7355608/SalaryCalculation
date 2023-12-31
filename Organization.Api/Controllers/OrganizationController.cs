﻿using AutoMapper;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Cors;
using Microsoft.AspNetCore.Mvc;
using Organization.App.Abstract;
using Organization.App.Commands;
using SalaryCalculation.Data.Enums;
using Serilog.Events;
using SerilogTimings;

namespace Organization.Api.Controllers;

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
        return GetAjaxResponse(IsValid, organization, Errors);
    }

    [AllowAnonymous]
    [HttpGet("all")]
    public async Task<IActionResult> GetOrganizationsAsync()
    {
        var organizations = await OrganizationCommandHandler.GetOrganizationsAsync();
        return GetAjaxResponse(IsValid, organizations, Errors);
    }

    [AllowAnonymous]
    [HttpGet("all/short")]
    public async Task<IActionResult> GetOrganizationsShortAsync()
    {
        var organizations = await OrganizationCommandHandler.GetOrganizationsShortAsync();
        return GetAjaxResponse(IsValid, organizations, Errors);
    }

    [AllowAnonymous]
    [HttpPost("create")]
    public async Task<IActionResult> CreateOrganizationAsync([FromBody] OrganizationCreateCommand command)
    {
        using var op = Operation.At(LogEventLevel.Debug).Begin("Organization {0} creating started", command.Name);
        await OrganizationCommandHandler.CreateOrganizationAsync(command);
        op.Complete();
        return GetAjaxResponse(IsValid, Errors);
    }

    [HttpPut("update/{id}")]
    public async Task<IActionResult> UpdateOrganizationAsync([FromRoute] int id,
        [FromBody] OrganizationUpdateCommand command)
    {
        using var op = Operation.At(LogEventLevel.Debug).Begin("Organization {0} updating started", command.Name);
        command.Id = id;
        var updated = await OrganizationCommandHandler.UpdateOrganizationAsync(command);

        op.Complete();
        return GetAjaxResponse(IsValid && updated, Errors);
    }

    [HttpDelete("delete/{id}")]
    public async Task<IActionResult> DeleteOrganizationAsync([FromRoute] int id)
    {
        using var op = Operation.At(LogEventLevel.Debug).Begin("Organization with id {0} deleting started", id);
        var deleted = await OrganizationCommandHandler.DeleteOrganizationAsync(id);
        op.Complete();
        return GetAjaxResponse(IsValid && deleted, Errors);
    }

    [HttpPut("{organizationId}/permissions/set")]
    public async Task<IActionResult> SetOrganizationPermissions([FromRoute] int organizationId,
        [FromBody] OrganizationPermissionUpdateCommand cmd)
    {
        cmd.OrganizationId = organizationId;
        var updated = await OrganizationCommandHandler.UpdateOrganizationPermissionsAsync(cmd);
        return GetAjaxResponse(IsValid && updated, Errors);
    }

    #region Organization Units
    
    [HttpGet("{organizationId}/units/{id}")]
    public async Task<IActionResult> GetOrganizationUnitAsync([FromRoute] int organizationId,
        [FromRoute] int id)
    {
        var unit = await OrganizationCommandHandler.GetOrganizationUnitAsync(organizationId, id);
        return GetAjaxResponse(IsValid, unit);
    }
    
    [HttpGet("{organizationId}/units")]
    public async Task<IActionResult> GetOrganizationUnitsAsync([FromRoute] int organizationId)
    {
        var units = await OrganizationCommandHandler.GetOrganizationUnitsAsync(organizationId);
        return GetAjaxResponse(IsValid, units);
    }
    
    [HttpGet("{organizationId}/units/short")]
    public async Task<IActionResult> GetOrganizationUnitsShortAsync([FromRoute] int organizationId)
    {
        var units = await OrganizationCommandHandler.GetOrganizationUnitsShortAsync(organizationId);
        return GetAjaxResponse(IsValid, units);
    }

    [HttpPost("{organizationId}/units/search")]
    public async Task<IActionResult> SearchOrganizationUnitsAsync([FromRoute] int organizationId,
        [FromBody] OrganizationUnitSearchCommand command)
    {
        using var op = Operation.At(LogEventLevel.Debug).Begin("Organization units searching started");
        var units = await OrganizationCommandHandler.SearchOrganizationUnitsAsync(command);
        op.Complete();
        return GetAjaxResponse(IsValid, units);
    }
    
    [HttpPost("units/create")]
    public async Task<IActionResult> CreateOrganizationUnitAsync([FromBody] OrganizationUnitCreateCommand command)
    {
        using var op = Operation.At(LogEventLevel.Debug).Begin("Organization unit {0} creating started", command.Name);
        await OrganizationCommandHandler.CreateOrganizationUnitAsync(command);
        op.Complete();
        return GetAjaxResponse(IsValid, Errors);
    }

    [HttpPut("units/update/{id}")]
    public async Task<IActionResult> UpdateOrganizationUnitAsync([FromRoute] int id,
        [FromBody] OrganizationUnitUpdateCommand command)
    {
        using var op = Operation.At(LogEventLevel.Debug).Begin("Organization unit {0} updating started", command.Name);
        command.Id = id;
        var updated = await OrganizationCommandHandler.UpdateOrganizationUnitAsync(command);

        op.Complete();
        return GetAjaxResponse(IsValid && updated, Errors);
    }

    [HttpDelete("{organizationId}/units/delete/{id}")]
    public async Task<IActionResult> DeleteOrganizationUnitAsync([FromRoute] int organizationId, [FromRoute] int id)
    {
        using var op = Operation.At(LogEventLevel.Debug).Begin("Organization with id {0} deleting started", id);
        var deleted = await OrganizationCommandHandler.DeleteOrganizationUnitAsync(organizationId, id);
        op.Complete();
        return GetAjaxResponse(IsValid && deleted, Errors);
    }

    #endregion

    #region Positions
    
    [HttpGet("{organizationId}/units/{organizationUnitId}/positions/{id}")]
    public async Task<IActionResult> GetPositionAsync([FromRoute] int organizationId, [FromRoute] int organizationUnitId,
        [FromRoute] int id)
    {
        var position = await OrganizationCommandHandler.GetPositionAsync(organizationId, organizationUnitId, id);
        return GetAjaxResponse(IsValid, position);
    }
    
    [HttpGet("{organizationId}/positions/{organizationUnitId:int?}")]
    public async Task<IActionResult> GetPositionsAsync([FromRoute] int organizationId, [FromRoute] int? organizationUnitId)
    {
        var position = await OrganizationCommandHandler.GetPositionsAsync(organizationId, organizationUnitId);
        return GetAjaxResponse(IsValid, position);
    }
    
    [HttpGet("{organizationId}/positions/short/{organizationUnitId:int?}")]
    public async Task<IActionResult> GetPositionsShortAsync([FromRoute] int organizationId, [FromRoute] int? organizationUnitId)
    {
        var position = await OrganizationCommandHandler.GetPositionsShortAsync(organizationId, organizationUnitId);
        return GetAjaxResponse(IsValid, position);
    }

    [HttpPost("{organizationId}/positions/search")]
    public async Task<IActionResult> SearchPositionAsync([FromRoute] int organizationId,
        [FromBody] PositionSearchCommand command)
    {
        using var op = Operation.At(LogEventLevel.Debug).Begin("Organization units searching started");
        command.OrganizationId = organizationId;
        var positions = await OrganizationCommandHandler.SearchPositionsAsync(command);
        op.Complete();
        return GetAjaxResponse(IsValid, positions);
    }
    
    [HttpPost("positions/create")]
    public async Task<IActionResult> CreatePositionAsync([FromBody] PositionCreateCommand command)
    {
        using var op = Operation.At(LogEventLevel.Debug).Begin("Organization unit {0} creating started", command.Name);
        await OrganizationCommandHandler.CreatePositionAsync(command);
        op.Complete();
        return GetAjaxResponse(IsValid, Errors);
    }

    [HttpPut("positions/update/{id}")]
    public async Task<IActionResult> UpdatePositionAsync([FromRoute] int id,
        [FromBody] PositionUpdateCommand command)
    {
        using var op = Operation.At(LogEventLevel.Debug).Begin("Organization unit {0} updating started", command.Name);
        command.Id = id;
        var updated = await OrganizationCommandHandler.UpdatePositionAsync(command);

        op.Complete();
        return GetAjaxResponse(IsValid && updated, Errors);
    }

    [HttpDelete("{organizationId}/units/{organizationUnitId}/positions/delete/{id}")]
    public async Task<IActionResult> DeletePositionAsync([FromRoute] int organizationId, [FromRoute] int organizationUnitId, [FromRoute] int id)
    {
        using var op = Operation.At(LogEventLevel.Debug).Begin("Organization with id {0} deleting started", id);
        var deleted = await OrganizationCommandHandler.DeletePositionAsync(organizationId, organizationUnitId, id);
        op.Complete();
        return GetAjaxResponse(IsValid && deleted, Errors);
    }

    #endregion
}