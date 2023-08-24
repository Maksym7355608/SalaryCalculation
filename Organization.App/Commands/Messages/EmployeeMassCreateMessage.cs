using SalaryCalculation.Data.BaseModels;

namespace Organization.App.Commands.Messages;

public class EmployeeMassCreateMessage : BaseMessage
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

public class EmployeeMassUpdateMessage : BaseMessage
{
    public int OrganizationId { get; set; }
    public IEnumerable<int> EmployeeIds { get; set; }
    public EmployeeMassUpdate UpdateModel { get; set; }
}

public class EmployeeMassDeleteMessage : BaseMessage
{
    public IEnumerable<int> EmployeeIds { get; set; }
    public IEnumerable<string> RollNumbers { get; set; }
    public int OrganizationId { get; set; }
    public int? OrganizationUnitId { get; set; }
    public int? PositionId { get; set; }
}