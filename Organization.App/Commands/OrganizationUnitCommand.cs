using SalaryCalculation.Data.BaseModels;

namespace Organization.App.Commands;

public class OrganizationUnitUpdateCommand : OrganizationUnitCreateCommand
{
    public int Id { get; set; }
}

public class OrganizationUnitCreateCommand : BaseCommand
{
    public int OrganizationId { get; set; }
    public string Name { get; set; }
    public int? OrganizationUnitId { get; set; }
}

public class OrganizationUnitSearchCommand : BaseCommand
{
    public int OrganizationId { get; set; }
    public string? Name { get; set; }
    public int? OrganizationUnitId { get; set; }
}