using MongoDB.Bson;
using SalaryCalculation.Data.BaseModels;

namespace Identity.Data.Entities;

public class Role : BaseMongoEntity<ObjectId>
{
    public string Name { get; set; }
    public string NormalizedName => Name.Normalize();
    public IEnumerable<int> Permissions { get; set; }

    public int OrganizationId { get; set; }
}