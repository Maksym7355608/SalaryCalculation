using MongoDB.Bson;
using SalaryCalculation.Data.BaseModels;

namespace Identity.App.Commands;

public class UserUpdateCommand : BaseCommand
{
    public string Id { get; set; }
}