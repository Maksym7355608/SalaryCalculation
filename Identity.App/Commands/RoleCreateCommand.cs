using SalaryCalculation.Data.BaseModels;
using SalaryCalculation.Data.Enums;

namespace Identity.App.Commands;

public class RoleCreateCommand : BaseCommand
{
    public string Name { get; set; }
    public int OrganizationId { get; set; }
    public IEnumerable<EPermission> Permissions { get; set; }
}