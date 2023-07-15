using AutoMapper;
using Microsoft.AspNetCore.Mvc;
using Identity.App.Abstract;
using Identity.App.Commands;
using Identity.Api.Commands;
using MongoDB.Bson;

namespace Identity.Api.Controllers;

[ApiController]
[Route("api/[controller]")]
public class IdentityController : BaseIdentityController
{
    public IdentityController(IMapper mapper, IIdentityCommandHandler identityCommandHandler,
        IRoleCommandHandler roleCommandHandler) : base(mapper, identityCommandHandler, roleCommandHandler)
    {
    }
    
    [HttpPost]
    public async Task<IActionResult> SignInAsync([FromBody] AuthentificateUserViewCommand command)
    {
        var token = await IdentityCommandHandler.AuthenticateAsync(command.Login, command.Password);
        if (string.IsNullOrWhiteSpace(token))
        {
            ModelState.AddModelError("authError", "Incorrect login or password");
            return RestAjaxResponse(IsValid, Errors);
        }
        return RestAjaxResponse(IsValid, data: token);
    }

    [HttpPost("create")]
    public async Task<IActionResult> CreateUserAsync([FromBody] UserCreateCommand command)
    {
        await IdentityCommandHandler.CreateUserAsync(command);
        return RestAjaxResponse(IsValid, Errors);
    }

    [HttpPut("update")]
    public async Task<IActionResult> UpdateUserAsync([FromBody] UserUpdateCommand command)
    {
        await IdentityCommandHandler.UpdateUserAsync(command);
        return RestAjaxResponse(IsValid, Errors);
    }
    
    [HttpDelete("delete")]
    public async Task<IActionResult> DeleteUserAsync([FromBody] ObjectId id)
    {
        await IdentityCommandHandler.DeleteUserAsync(id);
        return RestAjaxResponse(IsValid, Errors);
    }
    
    [HttpPost("addRole")]
    public async Task<IActionResult> AddUserRoleAsync([FromBody] ObjectId userId, [FromBody] ObjectId roleId)
    {
        await IdentityCommandHandler.AddRoleToUserAsync(userId, roleId);
        return RestAjaxResponse(IsValid, Errors);
    }
    
    [HttpPost("removeRole")]
    public async Task<IActionResult> RemoveUserRoleAsync([FromBody] ObjectId userId, [FromBody] ObjectId roleId)
    {
        await IdentityCommandHandler.RemoveRoleFromUserAsync(userId, roleId);
        return RestAjaxResponse(IsValid, Errors);
    }
}