namespace Dictionary.App.Dto;

public class BaseAmountDto
{
    public string Id { get; set; }
    public string Name { get; set; }
    public string ExpressionName { get; set; }
    public decimal Value { get; set; }
    public string Note { get; set; }
    public DateTime DateFrom { get; set; }
    public DateTime? DateTo { get; set; }
}