using SalaryCalculation.Data.Enums;

namespace Organization.App.Commands;

public class OrganizationPermissionUpdateCommand
{
    public int OrganizationId { get; set; }
    public IEnumerable<EPermission> Permissions { get; set; }
}