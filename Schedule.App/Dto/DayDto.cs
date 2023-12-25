using Schedule.Data.Enums;

namespace Schedule.App.Dto;

public class DayDto
{
    public DayOfWeek WeekDay { get; set; }

    public int Week { get; set; }
    public int DayOfCircle { get; set; }
}

public class TimeDto
{
    public string TimeValue => $"{Hour.ToString("D2")}:{Minutes.ToString("D2")}";
    public int Hour { get; set; }
    public int Minutes { get; set; }
}