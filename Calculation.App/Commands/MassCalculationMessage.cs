using SalaryCalculation.Data.BaseModels;

namespace Calculation.App.Commands;

public class MassCalculationMessage : BaseMessage
{
    public int Period { get; set; }
    public int OrganizationId { get; set; }
    public int? OrganizationUnitId { get; set; }
    public int? PositionId { get; set; }
    public int? RegimeId { get; set; }
}