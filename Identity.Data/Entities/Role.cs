namespace Identity.Data.Entities;

public class Role
{
    public int Id { get; set; }
    public string Name { get; set; }
    public string NormalizedName => Name.Normalize();
    public IEnumerable<int> Permissions { get; set; }

    public int OrganizationId { get; set; }
}