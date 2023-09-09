using SalaryCalculation.Data.BaseModels;

namespace Dictionary.App.Commands;

public class BaseAmountsSearchCommand : BaseCommand
{
    public string Name { get; set; }
    public string ExpressionName { get; set; }
}