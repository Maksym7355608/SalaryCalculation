using SalaryCalculation.Data.BaseModels;

namespace Dictionary.App.Commands;

public class FormulasSearchCommand : BaseCommand
{
    public string? Id { get; set; }
    public int OrganizationId { get; set; }
    public string? Name { get; set; }
    public string? ExpressionName { get; set; }
    public DateTime DateFrom { get; set; }
    public DateTime? DateTo { get; set; }
}