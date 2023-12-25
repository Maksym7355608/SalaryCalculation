namespace Schedule.App.Dto;

public class HoursDetailDto
{
    public decimal Summary { get; set; }
    public decimal Day { get; set; }
    public decimal Night { get; set; }
    public decimal Evening { get; set; }
    public bool Holiday { get; set; }
}

public class HoursDetailsDto
{
    public decimal Summary { get; set; }
    public decimal Day { get; set; }
    public decimal Night { get; set; }
    public decimal Evening { get; set; }
    public decimal HolidaySummary { get; set; }
    public decimal HolidayDay { get; set; }
    public decimal HolidayNight { get; set; }
    public decimal HolidayEvening { get; set; }
}