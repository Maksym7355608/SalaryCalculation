using MongoDB.Bson;
using SalaryCalculation.Data.BaseModels;

namespace Schedule.Data.Entities;

public class Calendar : BaseMongoEntity
{
    public ObjectId Id { get; set; }
    /// <summary>
    /// Enum - EWorkType
    /// </summary>
    public int WorkType { get; set; }
    /// <summary>
    /// Enum - EDayType
    /// </summary>
    public int DayType { get; set; }
    public DateTime Date { get; set; }
    public decimal Hours { get; set; }
    public decimal DayHours { get; set; }
    public decimal NightHours { get; set; }
    public decimal HolidayHours { get; set; }
    public int EmployeeId { get; set; }
    public int OrganizationId { get; set; }
}