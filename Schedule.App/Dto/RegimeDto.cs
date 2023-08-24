namespace Schedule.App.Dto;

public class RegimeDto
{
    public int Code { get; set; }

    public string Name { get; set; }

    public bool IsCircle { get; set; }

    public int DaysCount { get; set; }

    public WorkDayDetailDto WorkDayDetails { get; set; }

    public IEnumerable<DayDto> RestDayDetails { get; set; }
    
    public int RestDayCount => RestDayDetails.Count();

    public DateTime StartDateInCurrentYear { get; set; }

    public DateTime? StartDateInPreviousYear { get; set; }

    public DateTime? StartDateInNextYear { get; set; }
}