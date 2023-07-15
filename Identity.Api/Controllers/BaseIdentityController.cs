using AutoMapper;
using Identity.App.Abstract;
using Microsoft.AspNetCore.Mvc;
using SalaryCalculation.Shared.Extensions.Controllers;

namespace Identity.Api.Controllers;

public abstract class BaseIdentityController : BaseController
{
    protected readonly IIdentityCommandHandler IdentityCommandHandler;
    protected readonly IRoleCommandHandler RoleCommandHandler;

    public BaseIdentityController(IMapper mapper, IIdentityCommandHandler identityCommandHandler, IRoleCommandHandler roleCommandHandler) : base(mapper)
    {
        IdentityCommandHandler = identityCommandHandler;
        RoleCommandHandler = roleCommandHandler;
    }
}