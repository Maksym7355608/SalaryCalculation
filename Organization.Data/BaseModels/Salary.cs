namespace Organization.Data.BaseModels;

public class Salary
{
    public DateTime DateFrom { get; set; }
    public DateTime? DateTo { get; set; }
    public decimal Amount { get; set; }
}