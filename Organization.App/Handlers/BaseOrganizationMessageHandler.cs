using AutoMapper;
using Microsoft.Extensions.Logging;
using Organization.Data.Data;
using SalaryCalculation.Data;
using SalaryCalculation.Data.BaseHandlers;
using SalaryCalculation.Data.BaseModels;

namespace Organization.App.Handlers;

public abstract class BaseOrganizationMessageHandler<TMessage> : BaseMessageHandler<TMessage> where TMessage: BaseMessage
{
    protected new IOrganizationUnitOfWork Work;
    public BaseOrganizationMessageHandler(IOrganizationUnitOfWork work, ILogger<BaseOrganizationMessageHandler<TMessage>> logger, IMapper mapper) : base(work, logger, mapper)
    {
    }
}