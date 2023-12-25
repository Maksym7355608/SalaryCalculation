using MongoDB.Bson;
using SalaryCalculation.Data.BaseModels;
using Schedule.Data.BaseModels;

namespace Schedule.Data.Entities;

public class EmpDay : BaseMongoEntity<long>
{
    /// <summary>
    /// Enum - EDayType
    /// </summary>
    public int DayType { get; set; }
    public DateTime Date { get; set; }
    public HoursDetail Hours { get; set; }
    public int EmployeeId { get; set; }
    public int OrganizationId { get; set; }
}