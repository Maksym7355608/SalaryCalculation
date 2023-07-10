using MongoDB.Bson;
using SalaryCalculation.Data.BaseModels;

namespace Dictionary.Models;

public class Formula : BaseMongoEntity<ObjectId>
{
    public int OrganizationId { get; set; }
    public string Name { get; set; }
    public string ExpressionName { get; set; }
    public string Expression { get; set; }

    public DateTime DateFrom { get; set; }
    public DateTime? DateTo { get; set; }
}