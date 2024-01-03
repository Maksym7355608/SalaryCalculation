using AutoMapper;
using Calculation.Data;
using Microsoft.Extensions.Logging;
using SalaryCalculation.Data.BaseHandlers;

namespace Calculation.App.Handlers;

public class BaseCalculationCommandHandler : BaseCommandHandler
{
    protected new readonly ICalculationUnitOfWork Work;

    public BaseCalculationCommandHandler(ICalculationUnitOfWork work, ILogger<BaseCommandHandler> logger, IMapper mapper) : base(work, logger, mapper)
    {
        Work = work;
    }
}