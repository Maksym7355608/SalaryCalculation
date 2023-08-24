using MongoDB.Bson.Serialization.Attributes;
using SalaryCalculation.Data.BaseModels;
using Schedule.Data.BaseModels;

namespace Schedule.Data.Entities;

public class Regime : BaseMongoEntity<int>
{
    public int Code { get; set; }

    public string Name { get; set; }

    public bool IsCircle { get; set; }

    public int DaysCount { get; set; }

    public WorkDayDetail WorkDayDetails { get; set; }

    public IEnumerable<Day> RestDayDetails { get; set; }

    [BsonIgnore]
    public int RestDayCount => RestDayDetails.Count();

    public DateTime StartDateInCurrentYear { get; set; }

    public DateTime? StartDateInPreviousYear { get; set; }

    public DateTime? StartDateInNextYear { get; set; }

    public int OrganizationId { get; set; }
}