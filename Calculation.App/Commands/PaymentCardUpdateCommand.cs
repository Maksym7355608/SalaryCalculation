namespace Calculation.App.Commands;

public class PaymentCardUpdateCommand
{
    public int Id { get; set; }
    public DateTime? PaymentDate { get; set; }
    public int CalculationPeriod { get; set; }
    public decimal PayedAmount { get; set; }
    public decimal AccrualAmount { get; set; }
    public decimal MaintenanceAmount { get; set; }
    public IEnumerable<OperationUpdateCommand> OperationsCommand { get; set; }
}