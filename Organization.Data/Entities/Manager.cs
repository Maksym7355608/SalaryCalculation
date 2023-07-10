using MongoDB.Bson;
using Organization.Data.BaseModels;
using SalaryCalculation.Data.BaseModels;

namespace Organization.Data.Entities;

public class Manager : BaseMongoEntity<ObjectId>
{
    public long? RollNumber { get; set; }
    public Person Name { get; set; }
    public int OrganizationId { get; set; }
    public int EmployeeId { get; set; }
    public IEnumerable<Contact> Contacts { get; set; }
}