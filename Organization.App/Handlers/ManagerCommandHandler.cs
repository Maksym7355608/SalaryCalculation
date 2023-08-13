using AutoMapper;
using Microsoft.Extensions.Logging;
using MongoDB.Driver;
using Organization.App.Abstract;
using Organization.App.Commands;
using Organization.Data.Data;
using Organization.Data.Entities;
using SalaryCalculation.Data.BaseModels;
using SalaryCalculation.Shared.Common.Validation;
using Org = Organization.Data.Entities.Organization;

namespace Organization.App.Handlers;

public class ManagerCommandHandler : BaseOrganizationCommandHandler, IManagerCommandHandler
{
    public ManagerCommandHandler(IOrganizationUnitOfWork work, ILogger logger, IMapper mapper) : base(work, logger, mapper)
    {
    }

    public async Task<bool> AddManagerToOrganizationAsync(ManagerAddCommand command)
    {
        var manager = new Manager();
        if (command.IsBaseManager)
        {
            manager = await Work.GetCollection<Manager>(nameof(Manager))
                .Find(x => x.OrganizationId == 1)
                .FirstAsync();
        }
        else
        {
            if (!command.EmployeeId.HasValue) throw new ArgumentException("Employee id is empty");
            
            var employeeAsManager = await Work.GetCollection<Employee>(nameof(Employee))
                .Find(x => x.Organization.Id == command.OrganizationId && x.Id == command.EmployeeId.Value)
                .FirstOrDefaultAsync();
            
            if (employeeAsManager == null) throw new EntityNotFoundException(command.EmployeeId.Value.ToString());
            
            manager = Mapper.Map<Manager>(employeeAsManager);
        }
        var result = await Work.GetCollection<Org>(nameof(Org))
            .UpdateOneAsync(x => x.Id == command.OrganizationId, Builders<Org>.Update.Set(x => x.Manager, manager));

        return result.ModifiedCount > 0;
    }

    public async Task<bool> RemoveManagerFromOrganizationAsync(int organizationId)
    {
        var result = await Work.GetCollection<Org>(nameof(Org))
            .UpdateOneAsync(x => x.Id == organizationId, Builders<Org>.Update.Set(x => x.Manager, null));
        return result.ModifiedCount > 0;
    }
}