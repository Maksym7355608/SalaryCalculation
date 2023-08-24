using SalaryCalculation.Data.BaseModels;

namespace Organization.App.Commands;

public class OrganizationUnitUpdateCommand : BaseCommand
{
    public int Id { get; set; }
    public int OrganizationId { get; set; }
    public string Name { get; set; }
    public int? OrganizationUnitId { get; set; }
}

public class OrganizationUnitCreateCommand : OrganizationUnitUpdateCommand { }

public class OrganizationUnitSearchCommand : OrganizationUnitUpdateCommand { }