namespace Schedule.App.Dto;

public class WorkDayDetailDto
{
    public string DaysOfWeek { get; set; }

    public TimeDto StartTime { get; set; }

    public TimeDto EndTime { get; set; }

    public bool IsEndTimeNextDay { get; set; }

    public bool IsHolidayWork { get; set; }

    public bool? IsHolidayShort { get; set; }

    public TimeDto? StartTimeInHoliday { get; set; }

    public TimeDto? EndTimeInHoliday { get; set; }

    public bool? IsEndTimeInHolidayNextDay { get; set; }
    
    public bool IsLaunchPaid { get; set; }
    
    public int? LaunchTime { get; set; }
}