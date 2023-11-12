using SalaryCalculation.Data.BaseModels;

namespace Identity.App.Commands;

public class UserCreateCommand : BaseCommand
{
    public string Username { get; set; }
    public string Password { get; set; }
    public string FirstName { get; set; }
    public string MiddleName { get; set; }
    public string LastName { get; set; }
    public int OrganizationId { get; set; }
    public string PhoneNumber { get; set; }
    public string Email { get; set; }
}