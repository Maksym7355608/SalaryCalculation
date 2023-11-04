using AutoMapper;
using Identity.App.Abstract;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using SalaryCalculation.Shared.Common.Attributes;
using SalaryCalculation.Shared.Common.Controllers;

namespace Identity.Api.Controllers;

[ApiController]
[ServiceFilter(typeof(HandleExceptionAttribute))]
[Route("api/[controller]")]
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