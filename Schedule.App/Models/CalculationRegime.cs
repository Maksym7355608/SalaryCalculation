using Schedule.Data.BaseModels;

namespace Schedule.App.Models;

public class CalculationRegime
{
    public int RegimeId { get; set; }
    public int DaysCount { get; set; }
    public IEnumerable<WorkDayDetail> WorkDayDetails { get; set; } //пн-пт / 1-2
    public IEnumerable<Day> RestDays { get; set; } // сб-нд / 3-4
    public bool IsCircle { get; set; }
    public DateTime StartDateInCurrentYear { get; set; }
    public DateTime? StartDateInPreviousYear { get; set; }
    public DateTime? StartDateInNextYear { get; set; }
}