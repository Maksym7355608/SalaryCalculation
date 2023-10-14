using Calculation.App.Commands;
using Calculation.App.DtoModels;

namespace Calculation.App.Abstract;

public interface IOperationCommandHandler
{
    Task<OperationDto> GetOperationAsync(long id);
    Task<IEnumerable<OperationDto>> GetOperationsByEmployeeAsync(int employeeId, int? period);
    Task<IEnumerable<OperationDto>> SearchOperationsAsync(OperationsSearchCommand command);
    Task AddOperationAsync(OperationCreateCommand command);
    Task<bool> UpdateOperationAsync(OperationUpdateCommand command);
    Task<bool> DeleteOperationAsync(long id);
}