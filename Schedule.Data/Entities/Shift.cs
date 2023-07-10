using SalaryCalculation.Data.BaseModels;
using Schedule.Data.BaseModels;

namespace Schedule.Data.Entities;

public class Shift: BaseMongoEntity<int>
{
    public int Number { get; set; }
    public int WorkDaysCount { get; set; }
    public int RestDaysCount { get; set; }
    public IEnumerable<WorkDay> WorkDays { get; set; }
    public IEnumerable<int> RestDays { get; set; }
}