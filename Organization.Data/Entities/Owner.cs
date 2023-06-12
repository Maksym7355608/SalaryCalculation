using MongoDB.Bson;
using Organization.Data.BaseModels;

namespace Organization.Data.Entities;

public class Owner
{
    public int Id { get; set; }
    public Person Name { get; set; }
    public int OrganizationId { get; set; }
    public IEnumerable<Contact> Contacts { get; set; }
}