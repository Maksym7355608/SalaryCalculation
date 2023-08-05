using AutoMapper;
using Microsoft.Extensions.Logging;
using Organization.Data.Data;
using SalaryCalculation.Data.BaseHandlers;

namespace Organization.App.Handlers;

public class BaseOrganizationCommandHandler : BaseCommandHandler
{
    protected new IOrganizationUnitOfWork Work;
    public BaseOrganizationCommandHandler(IOrganizationUnitOfWork work, ILogger logger, IMapper mapper) : base(work, logger, mapper)
    {
    }
}