using AutoMapper;
using Calculation.App.Abstract;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using SalaryCalculation.Shared.Common.Attributes;
using SalaryCalculation.Shared.Common.Controllers;

namespace Calculation.API.Controllers;

[ApiController]
[ServiceFilter(typeof(HandleExceptionAttribute))]
[Authorize]
[Route("api/[controller]")]
public class BaseCalculationController : BaseController
{
    protected readonly IPaymentCardCommandHandler PaymentCardCommandHandler;
    protected readonly IOperationCommandHandler OperationCommandHandler;
    
    public BaseCalculationController(IPaymentCardCommandHandler paymentCardCommandHandler, 
        IOperationCommandHandler operationCommandHandler, IMapper mapper) : base(mapper)
    {
        PaymentCardCommandHandler = paymentCardCommandHandler;
        OperationCommandHandler = operationCommandHandler;
    }
}