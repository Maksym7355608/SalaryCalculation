using AutoMapper;
using Microsoft.AspNetCore.Mvc;
using Identity.App.Abstract;
using Identity.App.Commands;
using Identity.Api.Commands;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Cors;

namespace Identity.Api.Controllers;

[EnableCors("ApiCorsPolicy")]
[AllowAnonymous]
public class IdentityController : BaseIdentityController
{
    public IdentityController(IMapper mapper, IIdentityCommandHandler identityCommandHandler,
        IRoleCommandHandler roleCommandHandler) : base(mapper, identityCommandHandler, roleCommandHandler)
    {
    }

    [HttpPost]
    public async Task<IActionResult> SignInAsync([FromBody] AuthentificateUserViewCommand command)
    {
        var authModel = await IdentityCommandHandler.AuthenticateAsync(command.Login, command.Password);
        if (string.IsNullOrWhiteSpace(authModel.Token))
        {
            ModelState.AddModelError("authError", "Incorrect login or password");
            return GetAjaxResponse(IsValid, Errors);
        }

        return GetAjaxResponse(IsValid, authModel, Errors);
    }

    [HttpPost("create")]
    public async Task<IActionResult> CreateUserAsync([FromBody] UserCreateCommand command)
    {
        await IdentityCommandHandler.CreateUserAsync(command);
        return GetAjaxResponse(IsValid, Errors);
    }
}