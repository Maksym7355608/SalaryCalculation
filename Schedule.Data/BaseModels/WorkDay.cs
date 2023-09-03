namespace Schedule.Data.BaseModels;

public class WorkDayDetail
{
    public IEnumerable<Day> DaysOfWeek { get; set; }

    public Time StartTime { get; set; }

    public Time EndTime { get; set; }

    public bool IsEndTimeNextDay { get; set; }

    public bool IsHolidayWork { get; set; }

    public bool IsHolidayShort { get; set; }

    public Time StartTimeInHoliday { get; set; }

    public Time EndTimeInHoliday { get; set; }

    public bool IsEndTimeInHolidayNextDay { get; set; }
    
    public bool IsLaunchPaid { get; set; }
    
    public int LaunchTime { get; set; }
}

public class Day
{
    /// <summary>
    /// enum - DayOfWeek
    /// </summary>
    public int WeekDay { get; set; }

    public int Week { get; set; }
    public int DayOfCircle { get; set; }
}