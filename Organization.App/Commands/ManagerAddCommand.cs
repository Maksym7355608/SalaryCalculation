using SalaryCalculation.Data.BaseModels;

namespace Organization.App.Commands;

public class ManagerAddCommand : BaseCommand
{
    public int OrganizationId { get; set; }
    public int? EmployeeId { get; set; }
    public bool IsBaseManager { get; set; }
}