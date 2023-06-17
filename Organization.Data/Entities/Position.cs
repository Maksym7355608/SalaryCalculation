namespace Organization.Data.Entities;

public class Position
{
    public int Id { get; set; }
    public string Name { get; set; }
    public int OrganizationId { get; set; }
    public int OrganizationUnitId { get; set; }
}