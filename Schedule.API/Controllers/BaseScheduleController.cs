using AutoMapper;
using SalaryCalculation.Shared.Common.Controllers;
using Schedule.App.Abstract;

namespace Schedule.API.Controllers;

public class BaseScheduleController : BaseController
{
    protected readonly IScheduleCommandHandler ScheduleCommandHandler;
    protected readonly IScheduleReaderLogic ScheduleReaderLogic;
    
    public BaseScheduleController(IScheduleCommandHandler scheduleCommandHandler, IScheduleReaderLogic scheduleReaderLogic, IMapper mapper) : base(mapper)
    {
        ScheduleReaderLogic = scheduleReaderLogic;
        ScheduleCommandHandler = scheduleCommandHandler;
    }
}