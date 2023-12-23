using MongoDB.Bson;
using SalaryCalculation.Data.BaseModels;
using Schedule.Data.BaseModels;

namespace Schedule.Data.Entities;

public class PeriodCalendar : BaseMongoEntity<ObjectId>
{
    public int Period { get; set; }
    public int WorkDays { get; set; }
    public HoursDetails Hours { get; set; }
    public int VacationDays { get; set; }
    public int SickLeave { get; set; }
    public int EmployeeId { get; set; }
    public int OrganizationId { get; set; }
    public int RegimeId { get; set; }
}