using System.Collections;
using SalaryCalculation.Data.BaseModels;

namespace Organization.App.Commands;

public class EmployeeMassCreateCommand : BaseCommand
{
#if isDebug
    public bool IsGeneric { get; set; }
    public int? OrganizationUnitId { get; set; }
    public int? PositionId { get; set; }
    public int BatchCount { get; set; }
#endif
    public int OrganizationId { get; set; }
    public IEnumerable<EmployeeCreateCommand> Commands { get; set; }
}

public class EmployeeMassUpdateCommand : BaseCommand
{
    public int OrganizationId { get; set; }
    public IEnumerable<int> EmployeeIds { get; set; }
    public EmployeeMassUpdate UpdateModel { get; set; }
}

public class EmployeeMassUpdate
{
    public int? OrganizationUnitId { get; set; }
    public int? PositionId { get; set; }
}

public class EmployeeMassDeleteCommand : BaseCommand
{
    public IEnumerable<int> EmployeeIds { get; set; }
    public IEnumerable<string> RollNumbers { get; set; }
    public int OrganizationId { get; set; }
    public int? OrganizationUnitId { get; set; }
    public int? PositionId { get; set; }
}