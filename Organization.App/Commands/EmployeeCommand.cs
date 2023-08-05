using Organization.App.DtoModels;
using Organization.Data.Enums;
using SalaryCalculation.Data.BaseModels;

namespace Organization.App.Commands;

public class EmployeeUpdateCommand : BaseCommand
{
    public long RollNumber { get; set; }
    public string Name { get; set; }
    public string Surname { get; set; }
    public string Fatherly { get; set; }
    public DateTime DateFrom { get; set; }
    public decimal Salary { get; set; }
    public IEnumerable<EBenefit> Benefits { get; set; }
    public ESex Sex { get; set; }
    public EMarriedStatus MarriedStatus { get; set; }
    public BankDto BankAccount { get; set; }
    public IEnumerable<ContactDto> Contacts { get; set; }

    public IdNamePair OrganizationId { get; set; }
    public IdNamePair OrganizationUnitId { get; set; }
    public IdNamePair Position { get; set; }
}

public class EmployeeCreateCommand : EmployeeUpdateCommand
{
    
}

public class EmployeeSearchCommand : BaseCommand
{
    public int OrganizationId { get; set; }
    public long? RollNumber { get; set; }
    public DateTime? DateFrom { get; set; }
    public DateTime? DateTo { get; set; }
    public int? OrganizationUnit { get; set; }
    public int? Position { get; set; }
    public decimal? SalaryFrom { get; set; }
    public decimal? SalaryTo { get; set; }
}

