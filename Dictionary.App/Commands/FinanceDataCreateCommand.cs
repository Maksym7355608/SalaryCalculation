using SalaryCalculation.Data.BaseModels;

namespace Dictionary.App.Commands;

public class FinanceDataCreateCommand : BaseCommand
{
    public int Code { get; set; }
    public string Name { get; set; }
    public short Sign { get; set; }
    public int OrganizationId { get; set; }
    public string? Description { get; set; }
}