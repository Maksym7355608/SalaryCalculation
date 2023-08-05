using AutoMapper;
using Microsoft.AspNetCore.Mvc;
using Identity.App.Abstract;
using Identity.App.Commands;
using Identity.Api.Commands;
using Microsoft.AspNetCore.Authorization;
using MongoDB.Bson;
using SalaryCalculation.Shared.Common.Attributes;

namespace Identity.Api.Controllers;

[ApiController]
[HandleException]
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
            return BadRequest(new { IsValid, Errors });
        }

        return Ok(new { IsValid, Errors, data = token });
    }

    [HttpPost("create")]
    public async Task<IActionResult> CreateUserAsync([FromBody] UserCreateCommand command)
    {
        await IdentityCommandHandler.CreateUserAsync(command);
        return Ok(new { IsValid, Errors });
    }

    [Authorize]
    [HttpPut("update")]
    public async Task<IActionResult> UpdateUserAsync([FromBody] UserUpdateCommand command)
    {
        await IdentityCommandHandler.UpdateUserAsync(command);
        return Ok(new { IsValid, Errors });
    }

    [Authorize]
    [HttpDelete("delete")]
    public async Task<IActionResult> DeleteUserAsync([FromBody] ObjectId id)
    {
        await IdentityCommandHandler.DeleteUserAsync(id);
        return Ok(new { IsValid, Errors });
    }

    [Authorize]
    [HttpPost("addRole/{userId}/{roleId}")]
    public async Task<IActionResult> AddUserRoleAsync([FromRoute] ObjectId userId, [FromRoute] ObjectId roleId)
    {
        await IdentityCommandHandler.AddRoleToUserAsync(userId, roleId);
        return Ok(new { IsValid, Errors });
    }

    [Authorize]
    [HttpPost("removeRole/{userId}/{roleId}")]
    public async Task<IActionResult> RemoveUserRoleAsync([FromRoute] ObjectId userId, [FromRoute] ObjectId roleId)
    {
        await IdentityCommandHandler.RemoveRoleFromUserAsync(userId, roleId);
        return Ok(new { IsValid, Errors });
    }
}