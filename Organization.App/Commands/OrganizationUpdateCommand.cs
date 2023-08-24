using Organization.App.DtoModels;
using SalaryCalculation.Data.BaseModels;

namespace Organization.App.Commands;

public class OrganizationUpdateCommand : BaseCommand
{
    public int Id { get; set; }
    public string Code { get; set; }
    public string Name { get; set; }
    public long Edrpou { get; set; }
    public string Address { get; set; }
    public string FactAddress { get; set; }
    public IEnumerable<BankDto> BankAccounts { get; set; }

    public IdNamePair Chief { get; set; }
    public IdNamePair Accountant { get; set; }

    public IdNamePair? Manager { get; set; }
}