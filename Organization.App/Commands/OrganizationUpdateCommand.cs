using Organization.App.DtoModels;
using SalaryCalculation.Data.BaseModels;

namespace Organization.App.Commands;

public class OrganizationUpdateCommand : OrganizationCreateCommand
{
    public int Id { get; set; }
}