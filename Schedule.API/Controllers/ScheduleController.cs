using AutoMapper;
using Microsoft.AspNetCore.Mvc;
using SalaryCalculation.Shared.Common.Attributes;
using Schedule.App.Abstract;
using Schedule.App.Commands;
using SerilogTimings;

namespace Schedule.API.Controllers;

public class ScheduleController : BaseScheduleController
{
    public ScheduleController(IScheduleCommandHandler scheduleCommandHandler, IMapper mapper) : base(scheduleCommandHandler, mapper)
    {
    }

    #region Regime

    [HttpGet("regime/by-organization/{organizationId}")]
    public async Task<IActionResult> GetRegimesByOrganizationAsync([FromRoute] int organizationId)
    {
        var regimes = await ScheduleCommandHandler.GetRegimesAsync(organizationId);
        return GetAjaxResponse(IsValid, regimes, Errors);
    }

    [HttpGet("regime/by-organization/{organizationId}/short")]
    public async Task<IActionResult> GetRegimesByOrganizationShortAsync([FromRoute] int organizationId)
    {
        var regimes = await ScheduleCommandHandler.GetRegimesShortAsync(organizationId);
        return GetAjaxResponse(IsValid, regimes, Errors);
    }

    [HttpGet("regime/{id}")]
    public async Task<IActionResult> GetRegimeByIdAsync([FromRoute] int id)
    {
        var regime = await ScheduleCommandHandler.GetRegimeAsync(id);
        return GetAjaxResponse(IsValid, regime, Errors);
    }

    [HttpPost("regime/create")]
    public async Task<IActionResult> CreateRegimeAsync([FromBody] RegimeCreateCommand command)
    {
        var regimes = await ScheduleCommandHandler.CreateRegimeAsync(command);
        return GetAjaxResponse(IsValid && regimes, Errors);
    }

    [HttpPut("regime/update/{id}")]
    public async Task<IActionResult> UpdateRegimeAsync([FromRoute] int id, [FromBody] RegimeUpdateCommand command)
    {
        command.Id = id;
        var regimes = await ScheduleCommandHandler.UpdateRegimeAsync(command);
        return GetAjaxResponse(IsValid && regimes, Errors);
    }

    [HttpDelete("regime/delete/{id}")]
    public async Task<IActionResult> DeleteRegimeAsync([FromRoute] int id)
    {
        var regimes = await ScheduleCommandHandler.DeleteRegimeAsync(id);
        return GetAjaxResponse(IsValid && regimes, Errors);
    }

    #endregion

    #region Work days regime

    [HttpGet("regime/work-days/{id}")]
    public async Task<IActionResult> GetWorkDaysRegimeAsync([FromRoute] int id)
    {
        var workDays = await ScheduleCommandHandler.GetWorkDaysRegimeAsync(id);
        return GetAjaxResponse(IsValid, workDays, Errors);
    }
    
    [HttpPut("regime/{id}/work-days/update")]
    public async Task<IActionResult> GetWorkDaysRegimeAsync([FromRoute] int id, [FromBody] WorkDayRegimeUpdateCommand command)
    {
        command.RegimeId = id;
        var workDays = await ScheduleCommandHandler.UpdateWorkDayRegimeAsync(command);
        return GetAjaxResponse(IsValid && workDays, Errors);
    }

    #endregion

    #region PeriodCalendar

    [HttpGet("calendar/period/{empId}")]
    public async Task<IActionResult> GetPeriodsCalendarByEmployeeAsync([FromRoute] int empId)
    {
        var calendar = await ScheduleCommandHandler.GetPeriodsCalendarByEmployeeAsync(empId);
        return GetAjaxResponse(IsValid, calendar, Errors);
    }

    [HttpPost("calendar/period/search")]
    public async Task<IActionResult> SearchPeriodsCalendarsAsync([FromBody] PeriodCalendarSearchCommand command)
    {
        var calendars = await ScheduleCommandHandler.SearchPeriodsCalendarAsync(command);
        return GetAjaxResponse(IsValid, calendars, Errors);
    }
    
    [HttpGet("calendar/period/{period}/{empId}")]
    public async Task<IActionResult> GetPeriodsCalendarAsync([FromRoute] int period, [FromRoute] int empId)
    {
        var calendar = await ScheduleCommandHandler.GetPeriodCalendarAsync(empId, period);
        return GetAjaxResponse(IsValid, calendar, Errors);
    }

    #endregion

    #region Employee day
    
    [HttpGet("calendar/day/{id}/{period}")]
    public async Task<IActionResult> GetEmpDaysAsync([FromRoute] int id, [FromRoute]int period)
    {
        var days = await ScheduleCommandHandler.GetEmpDaysByEmployeeAsync(id, period);
        return GetAjaxResponse(IsValid, days, Errors);
    }

    [HttpPost("calendar/day/search")]
    public async Task<IActionResult> GetEmpDaysAsync([FromBody] WorkDaySearchCommand command)
    {
        var days = await ScheduleCommandHandler.GetWorkDaysAsync(command);
        return GetAjaxResponse(IsValid, days, Errors);
    }

    [HttpPost("calendar/day/set")]
    public async Task<IActionResult> SetEmpDayAsync([FromBody] WorkDayCreateCommand command)
    {
        var day = await ScheduleCommandHandler.SetWorkDayAsync(command);
        return GetAjaxResponse(IsValid && day, Errors);
    }
    #endregion

    #region Calculation

    [HttpGet("calendar/calculate/period/{period}/by-regime/{regimeId}/employee/{empId}")]
    public async Task<IActionResult> CalculatePeriodCalendarAsync([FromRoute] int period, [FromRoute] int regimeId,
        [FromRoute] int empId)
    {
        using var op = Operation.Begin("Calculating period calendar is started");
        var isCalc = await ScheduleCommandHandler.CalculatePeriodCalendarAsync(empId, period, regimeId);
        op.Complete();
        return GetAjaxResponse(IsValid && isCalc, Errors);
    }
    
    [HttpPost("calendar/calculate/period/mass")]
    public async Task<IActionResult> MassCalculatePeriodCalendarAsync([FromBody] PeriodCalendarMassCalculateCommand command)
    {
        using var op = Operation.Begin("Mass calculating period calendar is started");
        await ScheduleCommandHandler.MassCalculatePeriodCalendarAsync(command);
        op.Complete();
        return GetAjaxResponse(IsValid, Errors);
    }
    
    [HttpPost("calendar/calculate/day/mass")]
    public async Task<IActionResult> QuickSettingDaysAsync([FromBody] DaysSettingMessage msg)
    {
        using var op = Operation.Begin("Quick setting days is started");
        await ScheduleCommandHandler.QuickSettingDaysAsync(msg);
        op.Complete();
        return GetAjaxResponse(IsValid, Errors);
    }

    #endregion
}