namespace Schedule.App.Dto;

public class RegimeDto
{
    public int Code { get; set; }

    public string Name { get; set; }

    public bool IsCircle { get; set; }

    public int DaysCount { get; set; }

    public IEnumerable<WorkDayDetailDto> WorkDays { get; set; }

    public string RestDays { get; set; }
    
    public int RestDayCount => RestDays.Count();

    public DateTime StartDateInCurrentYear { get; set; }

    public DateTime? StartDateInPreviousYear { get; set; }

    public DateTime? StartDateInNextYear { get; set; }
}