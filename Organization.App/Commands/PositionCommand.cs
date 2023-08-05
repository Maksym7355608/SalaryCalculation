using SalaryCalculation.Data.BaseModels;

namespace Organization.App.Commands;

public class PositionUpdateCommand : BaseCommand
{
    public string Name { get; set; }
    public int OrganizationId { get; set; }
    public int OrganizationUnitId { get; set; }
}

public class PositionCreateCommand : PositionUpdateCommand { }

public class PositionSearchCommand : PositionUpdateCommand { }