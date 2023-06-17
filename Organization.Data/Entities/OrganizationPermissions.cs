using MongoDB.Bson;
using SalaryCalculation.Data.BaseModels;

namespace Organization.Data.Entities;

public class OrganizationPermissions : BaseMongoEntity
{
    public ObjectId Id { get; set; }
    public int OrganizationId { get; set; }
    public IEnumerable<int> Permissions { get; set; }
}