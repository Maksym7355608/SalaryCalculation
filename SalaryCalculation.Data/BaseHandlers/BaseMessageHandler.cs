using AutoMapper;
using Microsoft.Extensions.Logging;
using SalaryCalculation.Data.BaseModels;

namespace SalaryCalculation.Data.BaseHandlers;

public abstract class BaseMessageHandler<TMessage> where TMessage : BaseMessage
{
    protected IMapper Mapper { get; }
    protected IUnitOfWork Work { get; }
    protected ILogger Logger { get; }

    public BaseMessageHandler(IUnitOfWork work, ILogger logger, IMapper mapper)
    {
        Work = work;
        Mapper = mapper;
        Logger = logger;
    }

    public abstract Task HandleAsync(TMessage msg);
}