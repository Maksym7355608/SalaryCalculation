using AutoMapper;
using Microsoft.Extensions.Logging;
using SalaryCalculation.Data.BaseHandlers;
using Schedule.Data.Data;

namespace Schedule.App.Handlers;

public class BaseScheduleCommandHandler : BaseCommandHandler
{
    protected new IScheduleUnitOfWork Work;
    public BaseScheduleCommandHandler(IScheduleUnitOfWork work, ILogger<BaseScheduleCommandHandler> logger, IMapper mapper) : base(work, logger, mapper)
    {
        Work = work;
    }
}