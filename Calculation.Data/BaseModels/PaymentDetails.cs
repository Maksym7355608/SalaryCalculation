using SalaryCalculation.Data.BaseModels;

namespace Calculation.Data.BaseModels;

public class PaymentDetails
{
    public IdNamePair Operation { get; set; }
    public decimal Amount { get; set; }
    public decimal Hours { get; set; }
}