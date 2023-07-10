using System.Security.AccessControl;
using SalaryCalculation.Data.BaseModels;

namespace Organization.Data.Entities;

public class OrganizationUnit : BaseMongoEntity<int>
{
    public string Name { get; set; }
    public int OrganizationId { get; set; }
    public int? OrganizationUnitId { get; set; }
}