using MongoDB.Bson;
using SalaryCalculation.Data.BaseModels;

namespace Identity.Data.Entities;

public class User : BaseMongoEntity<ObjectId>
{
    public string UserName { get; set; }
    public string NormalizedUserName => UserName.Normalize();
    public string Email { get; set; }
    public string NormalizedEmail => Email.Normalize();
    public bool EmailConfirmed { get; set; }
    public string PasswordHash { get; set; }
    public string PhoneNumber { get; set; }
    public IEnumerable<ObjectId> Roles { get; set; }

    public string FirstName { get; set; }
    public string MiddleName { get; set; }
    public string LastName { get; set; }
}