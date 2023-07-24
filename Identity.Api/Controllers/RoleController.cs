using AutoMapper;
using Identity.App.Abstract;
using Identity.App.Commands;
using Identity.Data.Enums;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using MongoDB.Bson;
using SalaryCalculation.Shared.Common.Attributes;
using SalaryCalculation.Shared.Extensions.EnumExtensions;

namespace Identity.Api.Controllers;

[ApiController]
[HasPermission(EPermission.RoleControl)]
[Route("api/[controller]")]
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
        return RestAjaxResponse(IsValid, Errors);
    }
    
    [HttpPut("update/{id}")]
    public async Task<IActionResult> UpdateRoleAsync([FromRoute] ObjectId id, [FromBody] RoleUpdateCommand command)
    {
        await RoleCommandHandler.UpdateRole(id, command);
        return RestAjaxResponse(IsValid, Errors);
    }
    
    [HttpDelete("delete/{id}")]
    public async Task<IActionResult> DeleteRoleAsync([FromRoute] ObjectId id)
    {
        var result = await RoleCommandHandler.DeleteRole(id);
        return RestAjaxResponse(result && IsValid, Errors);
    }
}