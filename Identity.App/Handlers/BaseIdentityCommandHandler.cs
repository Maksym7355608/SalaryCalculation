using AutoMapper;
using Identity.App.Abstract;
using Identity.Data.Data;
using Microsoft.Extensions.Logging;
using SalaryCalculation.Data;
using SalaryCalculation.Data.BaseHandlers;

namespace Identity.App.Handlers;

public class BaseIdentityCommandHandler : BaseCommandHandler
{
    protected new IIdentityUnitOfWork Work;
    public BaseIdentityCommandHandler(IIdentityUnitOfWork work, ILogger logger, IMapper mapper) : base(work, logger, mapper)
    { }
}