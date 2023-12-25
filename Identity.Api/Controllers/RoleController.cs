using AutoMapper;
using Identity.App.Abstract;
using Identity.App.Commands;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Cors;
using Microsoft.AspNetCore.Mvc;
using MongoDB.Bson;
using SalaryCalculation.Shared.Common.Attributes;

namespace Identity.Api.Controllers;

[EnableCors("ApiAuthorizedCorsPolicy")]
[Authorize]
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
        command.Id = id;
        await RoleCommandHandler.UpdateRole(command);
        return GetAjaxResponse(IsValid, Errors);
    }

    [HttpDelete("delete/{id}")]
    public async Task<IActionResult> DeleteRoleAsync([FromRoute] ObjectId id)
    {
        var result = await RoleCommandHandler.DeleteRole(id);
        return GetAjaxResponse(IsValid && result, Errors);
    }
}