using MongoDB.Bson;

namespace Calculation.App.Commands;

public class PaymentCardCalculationCommand
{
    public int OrganizationId { get; set; }
    public int EmployeeId { get; set; }
    public int Period { get; set; }
    public ObjectId FormulaId { get; set; }
}