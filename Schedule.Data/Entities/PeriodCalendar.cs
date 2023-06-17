using MongoDB.Bson;
using SalaryCalculation.Data.BaseModels;

namespace Schedule.Data.Entities;

public class PeriodCalendar : BaseMongoEntity
{
    public ObjectId Id { get; set; }
    /// <summary>
    /// Enum - EWorkType
    /// </summary>
    public int WorkType { get; set; }
    public int Period { get; set; }
    public int WorkDays { get; set; }
    public decimal Hours { get; set; }
    public decimal DayHours { get; set; }
    public decimal NightHours { get; set; }
    public decimal HolidayHours { get; set; }
    public int EmployeeId { get; set; }
    public int OrganizationId { get; set; }
}