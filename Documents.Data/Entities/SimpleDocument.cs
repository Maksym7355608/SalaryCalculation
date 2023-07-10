using MongoDB.Bson;
using SalaryCalculation.Data.BaseModels;

namespace Documents.Data.Entities;

public class SimpleDocument : BaseMongoEntity<ObjectId>
{
    public Guid FileId { get; set; }
    public string Name { get; set; }
    public string FullName { get; set; }
    public DateTime CreateDate { get; set; }
    public int Period { get; set; }
    public int TemplateCode { get; set; }
    public ObjectId UserId { get; set; }
    public string DocumentType { get; set; }
    public int OrganizationId { get; set; }
    public int? OrganizationUnitId { get; set; }
    public int? PositionId { get; set; }
    public int? EmployeeId { get; set; }
}