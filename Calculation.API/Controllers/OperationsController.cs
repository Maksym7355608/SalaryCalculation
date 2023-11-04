using AutoMapper;
using Calculation.App.Abstract;
using Calculation.App.Commands;
using Calculation.App.DtoModels;
using Microsoft.AspNetCore.Mvc;

namespace Calculation.API.Controllers;

public class OperationsController : BaseCalculationController
{
    public OperationsController(IPaymentCardCommandHandler paymentCardCommandHandler, IOperationCommandHandler operationCommandHandler, IMapper mapper) : base(paymentCardCommandHandler, operationCommandHandler, mapper)
    {
    }
    
    [HttpGet("{id}")]
    public async Task<IActionResult> GetOperationAsync(long id)
    {
        var operation = await OperationCommandHandler.GetOperationAsync(id);
        return GetAjaxResponse(IsValid, operation, Errors);
    }

    [HttpGet("by-employee/{employeeId}/{period?}")]
    public async Task<ActionResult<IEnumerable<OperationDto>>> GetOperationsByEmployeeAsync(int employeeId, int? period)
    {
        var operations = await OperationCommandHandler.GetOperationsByEmployeeAsync(employeeId, period);
        return GetAjaxResponse(IsValid, operations, Errors);
    }

    [HttpPost("search")]
    public async Task<ActionResult<IEnumerable<OperationDto>>> SearchOperationsAsync(OperationsSearchCommand command)
    {
        var operations = await OperationCommandHandler.SearchOperationsAsync(command);
        if (operations == null)
            ModelState.AddModelError("", "Error searching operations");
        return GetAjaxResponse(IsValid, operations, Errors);
    }

    [HttpPost("create")]
    public async Task<IActionResult> AddOperationAsync(OperationCreateCommand command)
    {
        await OperationCommandHandler.AddOperationAsync(command);
        return GetAjaxResponse(IsValid, Errors);
    }

    [HttpPut("update")]
    public async Task<IActionResult> UpdateOperationAsync(OperationUpdateCommand command)
    {
        var res = await OperationCommandHandler.UpdateOperationAsync(command);
        return GetAjaxResponse(IsValid && res, Errors);
    }

    [HttpDelete("{id}")]
    public async Task<IActionResult> DeleteOperationAsync(long id)
    {
        var res = await OperationCommandHandler.DeleteOperationAsync(id);
        return GetAjaxResponse(IsValid && res, Errors);
    }

}