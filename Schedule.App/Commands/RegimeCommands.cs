using SalaryCalculation.Data.BaseModels;
using Schedule.App.Dto;

namespace Schedule.App.Commands;

public class RegimeCreateCommand : BaseCommand
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
    
    public int ShiftsCount { get; set; }
}

public class RegimeUpdateCommand : RegimeCreateCommand
{
    public int Id { get; set; }
}