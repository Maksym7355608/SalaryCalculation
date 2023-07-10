using MongoDB.Bson;
using SalaryCalculation.Data.BaseModels;

namespace Dictionary.Models;

public class FinanceData : BaseMongoEntity<ObjectId>
{
    public int Code { get; set; }
    public string Name { get; set; }
    public int OrganizationId { get; set; }
    public decimal? Percent { get; set; }
    public decimal? BaseValue { get; set; }
    public string Description { get; set; }
    public bool IsBaseAmount { get; set; }
}