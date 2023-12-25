using MongoDB.Bson;
using SalaryCalculation.Data.Enums;

namespace Identity.App.DtoModels;

public class RoleDto
{
    public ObjectId Id { get; set; }
    public string Name { get; set; }
    public IEnumerable<EPermission> Permissions { get; set; }
    public int OrganizationId { get; set; }
}