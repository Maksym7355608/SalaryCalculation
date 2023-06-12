using Organization.Data.BaseModels;

namespace Organization.Data.Entities;

/// <summary>
/// 
/// </summary>
public class Organization
{
    public int Id { get; set; }
    public string Code { get; set; }
    public string Name { get; set; }
    public long Edrpou { get; set; }
    public string Address { get; set; }
    public string FactAddress { get; set; }
    public Bank BankAccount { get; set; }
    public Owner Owner { get; set; }
    public Manager? Manager { get; set; }
}