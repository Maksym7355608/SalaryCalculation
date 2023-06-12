using MongoDB.Bson;
using Organization.Data.BaseModels;

namespace Organization.Data.Entities;

public class Manager
{
    public ObjectId Id { get; set; }
    public long? RollNumber { get; set; }
    public Person Name { get; set; }
    public int OrganizationId { get; set; }
    public IEnumerable<Contact> Contacts { get; set; }
}