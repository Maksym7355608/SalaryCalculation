using Organization.App.DtoModels;
using SalaryCalculation.Data.BaseModels;

namespace Organization.App.Commands;

public class OrganizationCreateCommand : BaseCommand
{
    public string Code { get; set; }
    public string Name { get; set; }
    public long Edrpou { get; set; }
    public string Address { get; set; }
    public string FactAddress { get; set; }
    public IEnumerable<BankDto> BankAccounts { get; set; }

    public string Chief { get; set; }
    public string Accountant { get; set; }

    public int? Manager { get; set; }
}