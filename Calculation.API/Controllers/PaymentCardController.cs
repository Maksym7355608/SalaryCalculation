using AutoMapper;
using Calculation.App.Abstract;
using Calculation.App.Commands;
using Microsoft.AspNetCore.Mvc;
using SerilogTimings;

namespace Calculation.API.Controllers;

public class PaymentCardController : BaseCalculationController
{
    public PaymentCardController(IPaymentCardCommandHandler paymentCardCommandHandler, IOperationCommandHandler operationCommandHandler, IMapper mapper) : base(paymentCardCommandHandler, operationCommandHandler, mapper)
    {
    }
    
    [HttpGet("{id}")]
    public async Task<IActionResult> GetPaymentCard(int id)
    {
        var paymentCard = await PaymentCardCommandHandler.GetPaymentCardAsync(id);
        return GetAjaxResponse(IsValid, paymentCard, Errors);
    }

    [HttpGet("employee/{employeeId}")]
    public async Task<IActionResult> GetPaymentCardsByEmployee(int employeeId, [FromQuery] int? period)
    {
        var paymentCards = await PaymentCardCommandHandler.GetPaymentCardsByEmployeeAsync(employeeId, period);
        return GetAjaxResponse(IsValid, paymentCards, Errors);
    }

    [HttpPost("search")]
    public async Task<IActionResult> SearchPaymentCards([FromBody] PaymentCardSearchCommand command)
    {
        var paymentCards = await PaymentCardCommandHandler.SearchPaymentCardsAsync(command);
        if(paymentCards == null)
            ModelState.AddModelError("", "Error searching payment cards");
        return GetAjaxResponse(IsValid, paymentCards, Errors);
    }

    [HttpPost("calculate")]
    public async Task<IActionResult> CalculatePaymentCard([FromBody] PaymentCardCalculationCommand command)
    {
        using var op = Operation.Begin("Calculating started");
        var result = await PaymentCardCommandHandler.CalculatePaymentCardAsync(command);
        op.Complete();
        return GetAjaxResponse(IsValid, result, Errors);
    }

    [HttpPut]
    public async Task<IActionResult> UpdatePaymentCard([FromBody] PaymentCardUpdateCommand command)
    {
        var success = await PaymentCardCommandHandler.UpdatePaymentCardsAsync(command);
        return GetAjaxResponse(IsValid && success, Errors);
    }

    [HttpDelete("{id}")]
    public async Task<IActionResult> DeletePaymentCard(int id)
    {
        var success = await PaymentCardCommandHandler.DeletePaymentCardAsync(id);
        return GetAjaxResponse(IsValid && success, Errors);
    }
}