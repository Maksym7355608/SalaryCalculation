using AutoMapper;
using Calculation.App.Abstract;
using Calculation.App.Commands;
using Microsoft.AspNetCore.Mvc;
using SalaryCalculation.Shared.Common.Attributes;

namespace Calculation.API.Controllers;

[ApiController]
[HandleException]
[Route("api/[controller]")]
public class PaymentCardController : BaseCalculationController
{
    public PaymentCardController(IPaymentCardCommandHandler paymentCardCommandHandler, IOperationCommandHandler operationCommandHandler, IMapper mapper) : base(paymentCardCommandHandler, operationCommandHandler, mapper)
    {
    }
    
    [HttpGet("{id}")]
    public async Task<IActionResult> GetPaymentCard(int id)
    {
        var paymentCard = await PaymentCardCommandHandler.GetPaymentCardAsync(id);
        if (paymentCard == null)
        {
            return BadRequest(new AjaxResponse(false, Errors));
        }
        return Ok(new AjaxResponse(true, paymentCard));
    }

    [HttpGet("employee/{employeeId}")]
    public async Task<IActionResult> GetPaymentCardsByEmployee(int employeeId, [FromQuery] int? period)
    {
        var paymentCards = await PaymentCardCommandHandler.GetPaymentCardsByEmployeeAsync(employeeId, period);
        if(paymentCards == null || !paymentCards.Any())
            return BadRequest(new AjaxResponse(false, Errors));
        return Ok(new AjaxResponse(true, paymentCards));
    }

    [HttpPost("search")]
    public async Task<IActionResult> SearchPaymentCards([FromBody] PaymentCardSearchCommand command)
    {
        var paymentCards = await PaymentCardCommandHandler.SearchPaymentCardsAsync(command);
        if(paymentCards == null || !paymentCards.Any())
            return BadRequest(new AjaxResponse(false, Errors));
        return Ok(new AjaxResponse(true, paymentCards));
    }

    [HttpPost("calculate")]
    public async Task<IActionResult> CalculatePaymentCard([FromBody] PaymentCardCalculationCommand command)
    {
        var result = await PaymentCardCommandHandler.CalculatePaymentCardAsync(command);
        if(string.IsNullOrWhiteSpace(result))
            return BadRequest(new AjaxResponse(false, Errors));
        return Ok(new AjaxResponse(true, result));
    }

    [HttpPut]
    public async Task<IActionResult> UpdatePaymentCard([FromBody] PaymentCardUpdateCommand command)
    {
        var success = await PaymentCardCommandHandler.UpdatePaymentCardsAsync(command);
        if(!success)
            return BadRequest(new AjaxResponse(false, Errors));
        return Ok(new AjaxResponse(true));
    }

    [HttpDelete("{id}")]
    public async Task<IActionResult> DeletePaymentCard(int id)
    {
        var success = await PaymentCardCommandHandler.DeletePaymentCardAsync(id);
        if(!success)
            return BadRequest(new AjaxResponse(false, Errors));
        return Ok(new AjaxResponse(true));
    }
}