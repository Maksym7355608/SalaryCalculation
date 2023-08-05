using AutoMapper;
using Microsoft.Extensions.Logging;
using Organization.App.Abstract;
using Organization.App.Commands;
using Organization.App.DtoModels;
using Organization.Data.Data;

namespace Organization.App.Handlers;

public class EmployeeCommandHandler : BaseOrganizationCommandHandler, IEmployeeCommandHandler
{
    public EmployeeCommandHandler(IOrganizationUnitOfWork work, ILogger logger, IMapper mapper) : base(work, logger, mapper)
    {
    }

    public async Task<EmployeeDto> GetEmployeeAsync(int employeeId)
    {
        throw new NotImplementedException();
    }

    public async Task<IEnumerable<EmployeeDto>> GetAllEmployeesAsync(int organizationId)
    {
        throw new NotImplementedException();
    }

    public async Task<IEnumerable<EmployeeDto>> SearchEmployeesAsync(EmployeeSearchCommand command)
    {
        throw new NotImplementedException();
    }

    public async Task CreateEmployeeAsync(EmployeeCreateCommand command)
    {
        throw new NotImplementedException();
    }

    public async Task<bool> UpdateEmployeeAsync(EmployeeUpdateCommand command)
    {
        throw new NotImplementedException();
    }

    public async Task<bool> DeleteEmployeeAsync(int employeeId)
    {
        throw new NotImplementedException();
    }

    public async Task<string> MassCreateEmployeesAsync(EmployeeMassCreateCommand command)
    {
        throw new NotImplementedException();
    }

    public async Task<string> MassCreateEmployeesAsync(EmployeeMassUpdateCommand command)
    {
        throw new NotImplementedException();
    }

    public async Task<string> MassDeleteEmployeesAsync(EmployeeMassDeleteCommand command)
    {
        throw new NotImplementedException();
    }
}