using Identity.App.DtoModels;
using MongoDB.Bson;
using SalaryCalculation.Data.BaseModels;

namespace Identity.App.Commands;

public class UserUpdateCommand : UserCreateCommand
{
    public string Id { get; set; }
}