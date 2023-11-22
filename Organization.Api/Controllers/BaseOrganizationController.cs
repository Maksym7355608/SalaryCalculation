using AutoMapper;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Cors;
using Microsoft.AspNetCore.Mvc;
using Organization.App.Abstract;
using SalaryCalculation.Shared.Common.Attributes;
using SalaryCalculation.Shared.Common.Controllers;

namespace Organization.Api.Controllers;

[ApiController]
[ServiceFilter(typeof(HandleExceptionAttribute))]
[Authorize]
[Route("api/[controller]")]
public class BaseOrganizationController : BaseController
{
    protected readonly IOrganizationCommandHandler OrganizationCommandHandler;
    protected readonly IManagerCommandHandler ManagerCommandHandler;
    protected readonly IEmployeeCommandHandler EmployeeCommandHandler;
    
    public BaseOrganizationController(IMapper mapper, IOrganizationCommandHandler organizationCommandHandler,
        IManagerCommandHandler managerCommandHandler, IEmployeeCommandHandler employeeCommandHandler) : base(mapper)
    {
        OrganizationCommandHandler = organizationCommandHandler;
        ManagerCommandHandler = managerCommandHandler;
        EmployeeCommandHandler = employeeCommandHandler;
    }
}