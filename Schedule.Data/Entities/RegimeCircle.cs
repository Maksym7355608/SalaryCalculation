using SalaryCalculation.Data.BaseModels;

namespace Schedule.Data.Entities;

public class RegimeCircle : BaseMongoEntity<int>
{
    public int DaysCount { get; set; }
    public int ShiftCount { get; set; }
    public IEnumerable<int> ShiftDetails { get; set; }
    public int RegimeId { get; set; }
}