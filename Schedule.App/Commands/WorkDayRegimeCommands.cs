using SalaryCalculation.Data.BaseModels;
using Schedule.App.Dto;

namespace Schedule.App.Commands;

public class WorkDayRegimeUpdateCommand : BaseCommand
{
    public int RegimeId { get; set; }
    public IEnumerable<WorkDayDetailDto> WorkDayDetails { get; set; }
    public IEnumerable<DayDto> RestDayDetails { get; set; }
}