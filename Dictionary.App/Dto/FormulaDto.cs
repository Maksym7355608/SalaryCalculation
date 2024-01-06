namespace Dictionary.App.Dto;

public class FormulaDto
{
    public string Id { get; set; }
    public int OrganizationId { get; set; }
    public string Name { get; set; }
    public string Condition { get; set; }
    public string ExpressionName { get; set; }
    public string Expression { get; set; }

    public DateTime DateFrom { get; set; }
    public DateTime? DateTo { get; set; }
    public int Code { get; set; }
}