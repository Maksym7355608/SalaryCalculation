using AutoMapper;
using Identity.App.Abstract;
using Identity.App.Commands;
using Microsoft.AspNetCore.Mvc;
using MongoDB.Bson;
using SalaryCalculation.Shared.Common.Attributes;

namespace Identity.Api.Controllers;

public class RoleController : BaseIdentityController
{
    public RoleController(IMapper mapper, IIdentityCommandHandler identityCommandHandler,
        IRoleCommandHandler roleCommandHandler) : base(mapper, identityCommandHandler, roleCommandHandler)
    {
    }

    [HttpPost("create")]
    public async Task<IActionResult> CreateRoleAsync([FromBody] RoleCreateCommand command)
    {
        await RoleCommandHandler.CreateRole(command);
        return GetAjaxResponse(IsValid, Errors);
    }

    [HttpPut("update/{id}")]
    public async Task<IActionResult> UpdateRoleAsync([FromRoute] ObjectId id, [FromBody] RoleUpdateCommand command)
    {
        await RoleCommandHandler.UpdateRole(id, command);
        return GetAjaxResponse(IsValid, Errors);
    }

    [HttpDelete("delete/{id}")]
    public async Task<IActionResult> DeleteRoleAsync([FromRoute] ObjectId id)
    {
        var result = await RoleCommandHandler.DeleteRole(id);
        return GetAjaxResponse(IsValid && result, Errors);
    }
}