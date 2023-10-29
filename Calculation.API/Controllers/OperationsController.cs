using AutoMapper;
using Calculation.App.Abstract;
using Calculation.App.Commands;
using Calculation.App.DtoModels;
using Microsoft.AspNetCore.Mvc;
using SalaryCalculation.Shared.Common.Attributes;

namespace Calculation.API.Controllers;

[ApiController]
[HandleException]
[Route("api/[controller]")]
public class OperationsController : BaseCalculationController
{
    public OperationsController(IPaymentCardCommandHandler paymentCardCommandHandler, IOperationCommandHandler operationCommandHandler, IMapper mapper) : base(paymentCardCommandHandler, operationCommandHandler, mapper)
    {
    }
    
    [HttpGet("{id}")]
    public async Task<IActionResult> GetOperationAsync(long id)
    {
        var operation = await OperationCommandHandler.GetOperationAsync(id);
        if (operation == null)
            return BadRequest(new AjaxResponse(false, Errors));
        return Ok(new AjaxResponse(ModelState.IsValid, operation));
    }

    [HttpGet("by-employee/{employeeId}/{period?}")]
    public async Task<ActionResult<IEnumerable<OperationDto>>> GetOperationsByEmployeeAsync(int employeeId, int? period)
    {
        var operations = await OperationCommandHandler.GetOperationsByEmployeeAsync(employeeId, period);
        if (operations == null || !operations.Any())
            return BadRequest(new AjaxResponse(false, Errors));
        return Ok(new AjaxResponse(ModelState.IsValid, operations));
    }

    [HttpPost("search")]
    public async Task<ActionResult<IEnumerable<OperationDto>>> SearchOperationsAsync(OperationsSearchCommand command)
    {
        var operations = await OperationCommandHandler.SearchOperationsAsync(command);
        if (operations == null)
            return BadRequest(new AjaxResponse(false, Errors));
        return Ok(new AjaxResponse(ModelState.IsValid, operations));
    }

    [HttpPost("create")]
    public async Task<IActionResult> AddOperationAsync(OperationCreateCommand command)
    {
        await OperationCommandHandler.AddOperationAsync(command);
        return Ok(new AjaxResponse(ModelState.IsValid, Errors));
    }

    [HttpPut("update")]
    public async Task<IActionResult> UpdateOperationAsync(OperationUpdateCommand command)
    {
        var res = await OperationCommandHandler.UpdateOperationAsync(command);
        if(!res)
            return BadRequest(new AjaxResponse(false, Errors));
        return Ok(new AjaxResponse(ModelState.IsValid, Errors));
    }

    [HttpDelete("{id}")]
    public async Task<IActionResult> DeleteOperationAsync(long id)
    {
        var res = await OperationCommandHandler.DeleteOperationAsync(id);
        if(!res)
            return BadRequest(new AjaxResponse(false, Errors));
        return Ok(new AjaxResponse(ModelState.IsValid, Errors));
    }

}