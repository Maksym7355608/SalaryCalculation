namespace Schedule.Data.BaseModels;

public class Time
{
    public string TimeValue => $"{Hour}:{Minutes}";
    public int Hour { get; set; }
    public int Minutes { get; set; }
}