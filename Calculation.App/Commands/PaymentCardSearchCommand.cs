namespace Calculation.App.Commands;

public class PaymentCardSearchCommand
{
    public int? Id { get; set; }
    public string? RollNumber { get; set; }
    public int? CalculationPeriod { get; set; }
    public int? OrganizationUnitId { get; set; }
    public int? PositionId { get; set; }
    public int OrganizationId { get; set; }
}