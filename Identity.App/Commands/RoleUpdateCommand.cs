using MongoDB.Bson;
using SalaryCalculation.Data.BaseModels;
using SalaryCalculation.Data.Enums;

namespace Identity.App.Commands;

public class RoleUpdateCommand : BaseCommand
{
    public ObjectId Id { get; set; }
    public string Name { get; set; }
    public int OrganizationId { get; set; }
    public IEnumerable<EPermission> Permissions { get; set; }
}