using Schedule.Data.Enums;

namespace Schedule.App.Dto;

public class TimeDto
{
    public string TimeValue => $"{Hour.ToString("D2")}:{Minutes.ToString("D2")}";
    public int Hour { get; set; }
    public int Minutes { get; set; }
}