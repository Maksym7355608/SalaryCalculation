using AutoMapper;
using Calculation.App.Abstract;
using SalaryCalculation.Shared.Common.Controllers;

namespace Calculation.API.Controllers;

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