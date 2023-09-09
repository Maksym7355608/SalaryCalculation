using SalaryCalculation.Data.BaseModels;
using Schedule.App.Dto;

namespace Schedule.App.Commands;

public class WorkDayRegimeUpdateCommand : BaseCommand
{
    public int WorkDayRegimeId { get; set; }
    public IEnumerable<DayDto> DaysOfWeek { get; set; }

    public TimeDto StartTime { get; set; }

    public TimeDto EndTime { get; set; }

    public bool IsEndTimeNextDay { get; set; }

    public bool IsHolidayWork { get; set; }

    public bool IsHolidayShort { get; set; }

    public TimeDto StartTimeInHoliday { get; set; }

    public TimeDto EndTimeInHoliday { get; set; }

    public bool IsEndTimeInHolidayNextDay { get; set; }
    
    public bool IsLaunchPaid { get; set; }
    
    public int LaunchTime { get; set; }
}