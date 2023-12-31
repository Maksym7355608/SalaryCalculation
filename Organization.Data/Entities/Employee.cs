using Organization.Data.BaseModels;
using SalaryCalculation.Data.BaseModels;

namespace Organization.Data.Entities;

public class Employee : BaseMongoEntity<int>
{
    public string RollNumber { get; set; }
    public Person Name { get; set; }
    public DateTime DateFrom { get; set; }
    public DateTime? DateTo { get; set; }
    public IEnumerable<Salary> Salaries { get; set; }
    public IEnumerable<int> Benefits { get; set; }
    public int Sex { get; set; }
    public int MarriedStatus { get; set; }
    public Bank BankAccount { get; set; }
    public IEnumerable<Contact> Contacts { get; set; }

    public IdNamePair Organization { get; set; }
    public IdNamePair OrganizationUnit { get; set; }
    public IdNamePair Position { get; set; }
    public int RegimeId { get; set; }
    public int ShiftNumber { get; set; }
}