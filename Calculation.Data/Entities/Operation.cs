using SalaryCalculation.Data.BaseModels;

namespace Calculation.Data.Entities;

public class Operation : BaseMongoEntity<long>
{
    public int Code { get; set; }
    public string Name { get; set; }
    public decimal Amount { get; set; }
    public string Note { get; set; }
    public int Period { get; set; }
    public int Sign { get; set; }
    public int EmployeeId { get; set; }
    public int OrganizationId { get; set; }
}