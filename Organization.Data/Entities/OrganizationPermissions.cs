using MongoDB.Bson;
using SalaryCalculation.Data.BaseModels;

namespace Organization.Data.Entities;

public class OrganizationPermissions : BaseMongoEntity<ObjectId>
{
    public int OrganizationId { get; set; }
    public IEnumerable<int> Permissions { get; set; }
}