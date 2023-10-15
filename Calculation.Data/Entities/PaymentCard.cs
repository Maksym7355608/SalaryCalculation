using SalaryCalculation.Data.BaseModels;

namespace Calculation.Data.Entities;

public class PaymentCard : BaseMongoEntity<int>
{
    public int OrganizationId { get; set; }
    public IdNamePair Employee { get; set; }
    public DateTime CalculationDate { get; set; }
    public DateTime? PaymentDate { get; set; }
    public int CalculationPeriod { get; set; }
    public decimal PayedAmount { get; set; }
    public decimal AccrualAmount { get; set; }
    public decimal MaintenanceAmount { get; set; }
    public IEnumerable<int> AccrualDetails { get; set; }
    public IEnumerable<int> MaintenanceDetails { get; set; }
}