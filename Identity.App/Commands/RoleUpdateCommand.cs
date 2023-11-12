using MongoDB.Bson;
using SalaryCalculation.Data.BaseModels;
using SalaryCalculation.Data.Enums;

namespace Identity.App.Commands;

public class RoleUpdateCommand : RoleCreateCommand
{
    public ObjectId Id { get; set; }
}