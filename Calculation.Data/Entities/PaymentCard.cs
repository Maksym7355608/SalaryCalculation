using Calculation.Data.BaseModels;
using SalaryCalculation.Data.BaseModels;

namespace Calculation.Data.Entities;

public class PaymentCard : BaseMongoEntity
{
    public int Id { get; set; }
    public int OrganizationId { get; set; }
    public int EmployeeId { get; set; }
    public DateTime CalculationDate { get; set; }
    public DateTime? PaymentDate { get; set; }
    public int CalculationPeriod { get; set; }
    public decimal PayedAmount { get; set; }
    public decimal AccrualAmount { get; set; }
    public decimal MaintenanceAmount { get; set; }
    public IEnumerable<PaymentDetails> AccrualDetails { get; set; }
    public IEnumerable<PaymentDetails> MaintenanceDetails { get; set; }
}