using MongoDB.Bson;

namespace Organization.Data.Entities;

public class OrganizationPermissions
{
    public ObjectId Id { get; set; }
    public int OrganizationId { get; set; }
    public IEnumerable<int> Permissions { get; set; }
}