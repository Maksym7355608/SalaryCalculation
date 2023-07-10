using SalaryCalculation.Data.BaseModels;

namespace Organization.Data.Entities;

public class Position : BaseMongoEntity<int>
{
    public string Name { get; set; }
    public int OrganizationId { get; set; }
    public int OrganizationUnitId { get; set; }
}