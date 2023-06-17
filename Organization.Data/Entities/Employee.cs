using Organization.Data.BaseModels;
using SalaryCalculation.Data.BaseModels;

namespace Organization.Data.Entities;

public class Employee : BaseMongoEntity
{
    public int Id { get; set; }
    public long RollNumber { get; set; }
    public Person Name { get; set; }
    public DateTime DateFrom { get; set; }
    public DateTime? DateTo { get; set; }
    public IEnumerable<Salary> Salaries { get; set; }
    public int? Benefit { get; set; }
    public int Sex { get; set; }
    public int MarriedStatus { get; set; }
    public Bank BankAccount { get; set; }

    public IdNamePair OrganizationId { get; set; }
    public IdNamePair OrganizationUnitId { get; set; }
    public IdNamePair Position { get; set; }
}