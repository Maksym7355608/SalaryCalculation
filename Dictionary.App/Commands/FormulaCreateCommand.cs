using SalaryCalculation.Data.BaseModels;

namespace Dictionary.App.Commands;

public class FormulaCreateCommand : BaseCommand
{
    public int OrganizationId { get; set; }
    public string Name { get; set; }
    public string ExpressionName { get; set; }
    public string Expression { get; set; }

    public DateTime DateFrom { get; set; }
    public DateTime? DateTo { get; set; }
}