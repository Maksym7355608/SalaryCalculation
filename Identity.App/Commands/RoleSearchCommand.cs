using MongoDB.Bson;
using SalaryCalculation.Data.BaseModels;

namespace Identity.App.Commands;

public class RoleSearchCommand : BaseCommand
{
    public ObjectId[] Ids { get; set; }
    public int OrganizationId { get; set; }
    public string SearchName { get; set; }
}