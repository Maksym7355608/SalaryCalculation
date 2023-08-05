using Organization.App.Commands;
using Organization.App.DtoModels;

namespace Organization.App.Abstract;

public interface IEmployeeCommandHandler
{
    Task<EmployeeDto> GetEmployeeAsync(int employeeId);
    Task<IEnumerable<EmployeeDto>> GetAllEmployeesAsync(int organizationId);
    Task<IEnumerable<EmployeeDto>> SearchEmployeesAsync(EmployeeSearchCommand command);

    Task CreateEmployeeAsync(EmployeeCreateCommand command);
    Task<bool> UpdateEmployeeAsync(EmployeeUpdateCommand command);
    Task<bool> DeleteEmployeeAsync(int employeeId);

    //progress
    Task<string> MassCreateEmployeesAsync(EmployeeMassCreateCommand command);
    Task<string> MassCreateEmployeesAsync(EmployeeMassUpdateCommand command);
    Task<string> MassDeleteEmployeesAsync(EmployeeMassDeleteCommand command);

}