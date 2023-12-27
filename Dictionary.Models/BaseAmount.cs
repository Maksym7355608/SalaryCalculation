using MongoDB.Bson;
using SalaryCalculation.Data.BaseModels;

namespace Dictionary.Models;

public class BaseAmount : BaseMongoEntity<ObjectId>
{
    public string Name { get; set; }
    public string ExpressionName { get; set; }
    public decimal Value { get; set; }
    public string Note { get; set; }
    public DateTime DateFrom { get; set; }
    public DateTime? DateTo { get; set; }
}