using SalaryCalculation.Data.BaseModels;

namespace Dictionary.App.Commands;

public class BaseAmountCreateCommand : BaseCommand
{
    public string Name { get; set; }
    public string ExpressionName { get; set; }
    public decimal Value { get; set; }
    public string Note { get; set; }
}