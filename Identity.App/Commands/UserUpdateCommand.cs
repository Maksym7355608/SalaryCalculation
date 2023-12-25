using Identity.App.DtoModels;
using MongoDB.Bson;
using SalaryCalculation.Data.BaseModels;

namespace Identity.App.Commands;

public class UserUpdateCommand : UserCreateCommand
{
    public ObjectId Id { get; set; }
    
    public IEnumerable<string> Roles { get; set; }
}