using AutoMapper;
using Organization.App.Abstract;
using SalaryCalculation.Shared.Common.Controllers;

namespace Organization.Api.Controllers;

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