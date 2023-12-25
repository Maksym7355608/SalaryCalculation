using Organization.App.DtoModels;
using Organization.Data.Enums;
using SalaryCalculation.Data.BaseModels;

namespace Organization.App.Commands;

public class EmployeeBaseCommand : BaseCommand
{
    public string Name { get; set; }
    public string Surname { get; set; }
    public string Fatherly { get; set; }
    public string NameGenitive { get; set; }
    public string ShortName { get; set; }
    public decimal Salary { get; set; }
    public IEnumerable<EBenefit> Benefits { get; set; }
    public EMarriedStatus MarriedStatus { get; set; }
    public BankDto BankAccount { get; set; }
    public string? Phone { get; set; }
    public string? Email { get; set; }
    public string? Telegram { get; set; }
    public int OrganizationUnitId { get; set; }
    public int PositionId { get; set; }
    public int RegimeId { get; set; }
    
    public int ShiftNumber { get; set; }
}

public class EmployeeUpdateCommand : EmployeeBaseCommand
{
    public int Id { get; set; }
    public DateTime? DateSalaryUpdated { get; set; }
    public DateTime? DateTo { get; set; }
}

public class EmployeeCreateCommand : EmployeeBaseCommand
{
    public string RollNumber { get; set; }
    public DateTime DateFrom { get; set; }
    public ESex Sex { get; set; }
    public int OrganizationId { get; set; }
}

public class EmployeeSearchCommand : BaseCommand
{
    public int OrganizationId { get; set; }
    public long? RollNumber { get; set; }
    public DateTime? DateFrom { get; set; }
    public DateTime? DateTo { get; set; }
    public int? OrganizationUnitId { get; set; }
    public int? PositionId { get; set; }
    public decimal? SalaryFrom { get; set; }
    public decimal? SalaryTo { get; set; }
}

