namespace Calculation.Data.Entities;

public class Operation
{
    public int Id { get; set; }
    public int Code { get; set; }
    public string Name { get; set; }
    public string Note { get; set; }
    public int OrganizationId { get; set; }
}