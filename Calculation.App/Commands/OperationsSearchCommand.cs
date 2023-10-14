namespace Calculation.App.Commands;

public class OperationsSearchCommand
{
    public int? Code { get; set; }
    public string? Name { get; set; }
    public int? Period { get; set; }
    public decimal? AmountFrom { get; set; }
    public decimal? AmountTo { get; set; }
    public int OrganizationId { get; set; }
}