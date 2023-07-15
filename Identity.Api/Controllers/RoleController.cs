using AutoMapper;
using Identity.App.Abstract;
using Identity.App.Commands;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using MongoDB.Bson;

namespace Identity.Api.Controllers;

[ApiController]
[Authorize]
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
    
    [HttpPut("update")]
    public async Task<IActionResult> UpdateRoleAsync([FromBody] ObjectId id, [FromBody] RoleUpdateCommand command)
    {
        await RoleCommandHandler.UpdateRole(id, command);
        return RestAjaxResponse(IsValid, Errors);
    }
    
    [HttpDelete("delete")]
    public async Task<IActionResult> DeleteRoleAsync([FromBody] ObjectId id)
    {
        var result = await RoleCommandHandler.DeleteRole(id);
        return RestAjaxResponse(result && IsValid, Errors);
    }
}