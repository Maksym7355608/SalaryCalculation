using Identity.App.DtoModels;
using MongoDB.Bson;
using SalaryCalculation.Data.BaseModels;

namespace Identity.App.Commands;

public class UserUpdateCommand : BaseCommand
{
    public string Id { get; set; }
    public string Username { get; set; }
    public string Password { get; set; }
    public string FirstName { get; set; }
    public string MiddleName { get; set; }
    public string LastName { get; set; }
    public string PhoneNumber { get; set; }
    public IEnumerable<RoleDto> Roles { get; set; }
}