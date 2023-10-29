using MongoDB.Bson;
using SalaryCalculation.Data.BaseModels;

namespace Calculation.App.Commands;

public class PaymentCardCalculationCommand
{
    public int OrganizationId { get; set; }
    public int EmployeeId { get; set; }
    public int Period { get; set; }
    public ObjectId FormulaId { get; set; }
}

public class CalculationSalaryMessage : BaseMessage
{
    public int OrganizationId { get; set; }
    public int EmployeeId { get; set; }
    public int Period { get; set; }
    public ObjectId FormulaId { get; set; }
}