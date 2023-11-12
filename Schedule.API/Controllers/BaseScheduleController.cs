using AutoMapper;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using SalaryCalculation.Shared.Common.Attributes;
using SalaryCalculation.Shared.Common.Controllers;
using Schedule.App.Abstract;

namespace Schedule.API.Controllers;

[ApiController]
[HandleException]
[Authorize]
[Route("api/[controller]")]
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