namespace Calculation.App.Commands;

public class PaymentCardSearchCommand
{
    public int? Id { get; set; }
    public int? CalculationPeriod { get; set; }
    public int OrganizationId { get; set; }
    public string[]? EmployeeNumbers { get; set; }
}