using Organization.Data.BaseModels;
using SalaryCalculation.Data.BaseModels;

namespace Organization.Data.Entities;

/// <summary>
/// Організація
/// </summary>
public class Organization : BaseMongoEntity<int>
{
    public string Code { get; set; }
    public string Name { get; set; }
    public long Edrpou { get; set; }
    public string Address { get; set; }
    public string FactAddress { get; set; }
    public IEnumerable<Bank> BankAccounts { get; set; }

    public IdNamePair Chief { get; set; }
    public IdNamePair Accountant { get; set; }

    public Manager? Manager { get; set; }
}