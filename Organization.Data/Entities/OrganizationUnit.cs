namespace Organization.Data.Entities;

public class OrganizationUnit
{
    public int Id { get; set; }
    public string Name { get; set; }
    public int OrganizationId { get; set; }
    
}