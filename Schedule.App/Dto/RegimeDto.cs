namespace Schedule.App.Dto;

public class RegimeDto
{
    public int Code { get; set; }

    public string Name { get; set; }

    public bool IsCircle { get; set; }

    public int DaysCount { get; set; }

    public IEnumerable<WorkDayDetailDto> WorkDayDetails { get; set; }

    public IEnumerable<DayDto> RestDayDetails { get; set; }
    
    public int RestDayCount => RestDayDetails.Count();

    public DateTime StartDateInCurrentYear { get; set; }

    public DateTime? StartDateInPreviousYear { get; set; }

    public DateTime? StartDateInNextYear { get; set; }
}

public class CalculationRegimeDto
{
    public int RegimeId { get; set; }
    public int DaysCount { get; set; }
    public IEnumerable<WorkDayDetailDto> WorkDayDetails { get; set; } //пн-пт / 1-2
    public IEnumerable<DayDto> RestDays { get; set; } // сб-нд / 3-4
    public bool IsCircle { get; set; }
    public DateTime StartDateInCurrentYear { get; set; }
    public DateTime? StartDateInPreviousYear { get; set; }
    public DateTime? StartDateInNextYear { get; set; }
}