using SalaryCalculation.Data.BaseModels;

namespace Dictionary.App.Commands;

public class FinanceDataSearchCommand : BaseCommand
{
    public int OrganizationId { get; set; }
    public IEnumerable<int> Codes { get; set; }
    public string Name { get; set; }
    public bool IsBaseAmount { get; set; }
}