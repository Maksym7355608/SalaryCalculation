using SalaryCalculation.Data.BaseModels;
using SalaryCalculation.Data.Enums;

namespace Identity.App.DtoModels;

public class AuthorizedUserDto
{
    public string Username { get; set; }
    public string Email { get; set; }
    public string FirstName { get; set; }
    public string MiddleName { get; set; }
    public string LastName { get; set; }

    public int OrganizationId { get; set; }
    
    public int[] Permissions { get; set; }
    public IdNamePair<string, string>[] Roles { get; set; }
}