using SalaryCalculation.Data.BaseModels;

namespace Schedule.Data.Entities;

public class Regime : BaseMongoEntity<int>
{
    public int Code { get; set; }

    public string Name { get; set; }

    public bool IsCircle { get; set; }

    public IEnumerable<int> RegimeCircleIds { get; set; }

    public DateTime StartDateInCurrentYear { get; set; }

    public DateTime StartDateInPreviousYear { get; set; }

    public DateTime? StartDateInNextYear { get; set; }
}