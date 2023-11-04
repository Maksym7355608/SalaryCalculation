using AutoMapper;
using Microsoft.AspNetCore.Mvc;
using Identity.App.Abstract;
using Identity.App.Commands;
using Identity.Api.Commands;
using Microsoft.AspNetCore.Authorization;
using MongoDB.Bson;
using SalaryCalculation.Shared.Common.Attributes;

namespace Identity.Api.Controllers;

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
            return GetAjaxResponse(IsValid, Errors);
        }

        return GetAjaxResponse(IsValid, token, Errors);
    }

    [HttpPost("create")]
    public async Task<IActionResult> CreateUserAsync([FromBody] UserCreateCommand command)
    {
        await IdentityCommandHandler.CreateUserAsync(command);
        return GetAjaxResponse(IsValid, Errors);
    }

    [Authorize]
    [HttpPut("update")]
    public async Task<IActionResult> UpdateUserAsync([FromBody] UserUpdateCommand command)
    {
        await IdentityCommandHandler.UpdateUserAsync(command);
        return GetAjaxResponse(IsValid, Errors);
    }

    [Authorize]
    [HttpDelete("delete")]
    public async Task<IActionResult> DeleteUserAsync([FromBody] ObjectId id)
    {
        await IdentityCommandHandler.DeleteUserAsync(id);
        return GetAjaxResponse(IsValid, Errors);
    }

    [Authorize]
    [HttpPost("addRole/{userId}/{roleId}")]
    public async Task<IActionResult> AddUserRoleAsync([FromRoute] ObjectId userId, [FromRoute] ObjectId roleId)
    {
        await IdentityCommandHandler.AddRoleToUserAsync(userId, roleId);
        return GetAjaxResponse(IsValid, Errors);
    }

    [Authorize]
    [HttpPost("removeRole/{userId}/{roleId}")]
    public async Task<IActionResult> RemoveUserRoleAsync([FromRoute] ObjectId userId, [FromRoute] ObjectId roleId)
    {
        await IdentityCommandHandler.RemoveRoleFromUserAsync(userId, roleId);
        return GetAjaxResponse(IsValid, Errors);
    }
}