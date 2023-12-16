using Organization.Data.Enums;
using SalaryCalculation.Data.BaseModels;

namespace Organization.App.DtoModels;

public class EmployeeDto
{
    public int Id { get; set; }
    public long RollNumber { get; set; }
    public PersonDto Name { get; set; }
    public DateTime DateFrom { get; set; }
    public DateTime? DateTo { get; set; }
    public IEnumerable<SalaryDto> Salaries { get; set; }
    public IEnumerable<EBenefit> Benefits { get; set; }
    public ESex Sex { get; set; }
    public EMarriedStatus MarriedStatus { get; set; }
    public BankDto BankAccount { get; set; }
    public IEnumerable<ContactDto> Contacts { get; set; }

    public IdNamePair Organization { get; set; }
    public IdNamePair OrganizationUnit { get; set; }
    public IdNamePair Position { get; set; }
    public int RegimeId { get; set; }
}

public class SalaryDto
{
    public DateTime DateFrom { get; set; }
    public DateTime? DateTo { get; set; }
    public decimal Amount { get; set; }
}

public class PersonDto
{
    public string FirstName { get; set; }
    public string LastName { get; set; }
    public string MiddleName { get; set; }
    public string ShortName { get; set; }
    public string NameGenitive { get; set; }
}

public class ContactDto
{
    public int Kind { get; set; }
    public string Value { get; set; }
}

