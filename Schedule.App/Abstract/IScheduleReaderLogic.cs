using Schedule.App.Commands;

namespace Schedule.App.Abstract;

public interface IScheduleReaderLogic
{
    Task<bool> CalculatePeriodCalendarAsync(int employeeId, int period, int regimeId);
    Task<string> MassCalculatePeriodCalendarAsync(PeriodCalendarMassCalculateCommand command);
    Task<bool> QuickSettingWorkDays(WorkDaysSettingFilter filter);
    Task<bool> QuickSettingRestDays(RestDaysSettingFilter filter);
}