using AutoMapper;
using Identity.App.Abstract;
using Identity.App.Commands;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Cors;
using Microsoft.AspNetCore.Mvc;
using MongoDB.Bson;

namespace Identity.Api.Controllers;

[EnableCors("ApiAuthorizedCorsPolicy")]
[Authorize]
public class UserController : BaseIdentityController
{
    public UserController(IMapper mapper, IIdentityCommandHandler identityCommandHandler, IRoleCommandHandler roleCommandHandler) : base(mapper, identityCommandHandler, roleCommandHandler)
    {
    }
    
    [HttpPut("update/{id}")]
    public async Task<IActionResult> UpdateUserAsync([FromRoute] ObjectId id, [FromBody] UserUpdateCommand command)
    {
        command.Id = id;
        await IdentityCommandHandler.UpdateUserAsync(command);
        return GetAjaxResponse(IsValid, Errors);
    }
    
    [HttpDelete("delete/{id}")]
    public async Task<IActionResult> DeleteUserAsync([FromRoute] ObjectId id)
    {
        await IdentityCommandHandler.DeleteUserAsync(id);
        return GetAjaxResponse(IsValid, Errors);
    }

    [HttpPost("addRole/{userId}/{roleId}")]
    public async Task<IActionResult> AddUserRoleAsync([FromRoute] ObjectId userId, [FromRoute] ObjectId roleId)
    {
        await IdentityCommandHandler.AddRoleToUserAsync(userId, roleId);
        return GetAjaxResponse(IsValid, Errors);
    }

    [HttpPost("removeRole/{userId}/{roleId}")]
    public async Task<IActionResult> RemoveUserRoleAsync([FromRoute] ObjectId userId, [FromRoute] ObjectId roleId)
    {
        await IdentityCommandHandler.RemoveRoleFromUserAsync(userId, roleId);
        return GetAjaxResponse(IsValid, Errors);
    }
}