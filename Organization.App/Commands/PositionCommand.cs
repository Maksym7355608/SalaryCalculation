using SalaryCalculation.Data.BaseModels;

namespace Organization.App.Commands;

public class PositionUpdateCommand : PositionCreateCommand
{
    public int Id { get; set; }
}

public class PositionCreateCommand : BaseCommand
{
    public string Name { get; set; }
    public int OrganizationId { get; set; }
    public int OrganizationUnitId { get; set; }
}

public class PositionSearchCommand : PositionUpdateCommand { }