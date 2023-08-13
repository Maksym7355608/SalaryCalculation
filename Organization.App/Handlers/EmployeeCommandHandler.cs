using AutoMapper;
using Microsoft.Extensions.Logging;
using MongoDB.Bson;
using MongoDB.Driver;
using Organization.App.Abstract;
using Organization.App.Commands;
using Organization.App.Commands.Messages;
using Organization.App.DtoModels;
using Organization.Data.Data;
using Organization.Data.Entities;
using Progress.App;
using SalaryCalculation.Shared.Common.Validation;

namespace Organization.App.Handlers;

public class EmployeeCommandHandler : BaseOrganizationCommandHandler, IEmployeeCommandHandler
{
    public EmployeeCommandHandler(IOrganizationUnitOfWork work, ILogger logger, IMapper mapper) : base(work, logger, mapper)
    {
    }

    public async Task<EmployeeDto> GetEmployeeAsync(int employeeId)
    {
        var employee = await Work.GetCollection<Employee>()
            .Find(x => x.Id == employeeId)
            .FirstOrDefaultAsync();
        if (employee == null) throw new EntityNotFoundException(employeeId.ToString());
        return Mapper.Map<EmployeeDto>(employee);
    }

    public async Task<IEnumerable<EmployeeDto>> GetAllEmployeesAsync(int organizationId)
    {
        var employees = await Work.GetCollection<Employee>()
            .Find(x => x.Organization.Id == organizationId)
            .ToListAsync();
        return Mapper.Map<IEnumerable<EmployeeDto>>(employees);
    }

    public async Task<IEnumerable<EmployeeDto>> SearchEmployeesAsync(EmployeeSearchCommand command)
    {
        var employees = await Work.GetCollection<Employee>()
            .Find(GetEmployeesSearchFilter(command))
            .ToListAsync();
        return Mapper.Map<IEnumerable<EmployeeDto>>(employees);
    }

    private FilterDefinition<Employee> GetEmployeesSearchFilter(EmployeeSearchCommand filter)
    {
        var filterBuilder = Builders<Employee>.Filter;

        var filterDefinition = new List<FilterDefinition<Employee>>()
        {
            filterBuilder.Eq(x => x.Organization.Id, filter.OrganizationId)
        };
        if(filter.OrganizationUnitId.HasValue)
            filterDefinition.Add(filterBuilder.Eq(x => x.OrganizationUnit.Id, filter.OrganizationUnitId.Value));
        if(filter.PositionId.HasValue)
            filterDefinition.Add(filterBuilder.Eq(x => x.Position.Id, filter.PositionId.Value));
        if(filter.DateFrom.HasValue)
            filterDefinition.Add(filterBuilder.Gte(x => x.DateFrom, filter.DateFrom.Value));
        if(filter.DateTo.HasValue)
            filterDefinition.Add(filterBuilder.Lte(x => x.DateFrom, filter.DateTo.Value));
        if(filter.SalaryFrom.HasValue)
            filterDefinition.Add(filterBuilder.Gte(x => x.Salaries.Last().Amount, filter.SalaryFrom.Value));
        if(filter.SalaryTo.HasValue)
            filterDefinition.Add(filterBuilder.Lte(x => x.Salaries.Last().Amount, filter.SalaryTo.Value));

        return filterBuilder.And(filterDefinition);
    }

    public async Task CreateEmployeeAsync(EmployeeCreateCommand command)
    {
        var employee = Mapper.Map<Employee>(command);
        if (Work.GetCollection<Employee>().Find(x => x.Organization.Id == command.Organization.Id
                                                     && x.OrganizationUnit.Id == command.OrganizationUnit.Id
                                                     && x.Position.Id == command.Position.Id
                                                     && x.RollNumber == command.RollNumber)
            .Any()) throw new DuplicateBsonMemberMapAttributeException("Employee with the same character exist");
        await Work.GetCollection<Employee>().InsertOneAsync(employee);
    }

    public async Task<bool> UpdateEmployeeAsync(EmployeeUpdateCommand command)
    {
        var employee = Mapper.Map<Employee>(command);
        if (!Work.GetCollection<Employee>().Find(x => x.Id == command.Id).Any()) 
            throw new EntityNotFoundException(command.Id.ToString());
        var result = await Work.GetCollection<Employee>().UpdateOneAsync(x => x.Id == command.Id, Builders<Employee>.Update.Set(x => x, employee));
        return result.ModifiedCount > 0;
    }

    public async Task<bool> DeleteEmployeeAsync(int employeeId)
    {
        var result = await Work.GetCollection<Employee>()
            .DeleteOneAsync(x => x.Id == employeeId);
        return result.DeletedCount > 0;
    }

    public async Task<string> MassCreateEmployeesAsync(EmployeeMassCreateCommand command)
    {
        var progressCmd = new ProgressCreateMessage()
        {
            ProgressId = Guid.NewGuid().ToString()
        }; //TODO
        await Work.MessageBroker.PublishAsync(progressCmd);
        var message = new EmployeeMassCreateMessage()
        {
#if isDebug
            IsGeneric = command.IsGeneric,
            OrganizationUnitId = command.OrganizationUnitId,
            PositionId = command.PositionId,
            BatchCount = command.BatchCount,
#endif
            OrganizationId = command.OrganizationId,
            Commands = command.Commands
        };

        await Work.MessageBroker.PublishAsync(message);

        return progressCmd.ProgressId;
    }

    public async Task<string> MassUpdateEmployeesAsync(EmployeeMassUpdateCommand command)
    {
        var progressCmd = new ProgressCreateMessage()
        {
            ProgressId = Guid.NewGuid().ToString()
        }; //TODO
        await Work.MessageBroker.PublishAsync(progressCmd);
        var message = new EmployeeMassUpdateMessage()
        {
            OrganizationId = command.OrganizationId,
            EmployeeIds = command.EmployeeIds,
            UpdateModel = command.UpdateModel
        };

        await Work.MessageBroker.PublishAsync(message);

        return progressCmd.ProgressId;
    }

    public async Task<string> MassDeleteEmployeesAsync(EmployeeMassDeleteCommand command)
    {
        var progressCmd = new ProgressCreateMessage()
        {
            ProgressId = Guid.NewGuid().ToString()
        }; //TODO
        await Work.MessageBroker.PublishAsync(progressCmd);
        var message = new EmployeeMassDeleteMessage()
        {
            OrganizationId = command.OrganizationId,
            EmployeeIds = command.EmployeeIds,
            PositionId = command.PositionId,
            OrganizationUnitId = command.OrganizationUnitId,
            RollNumbers = command.RollNumbers,
        };

        await Work.MessageBroker.PublishAsync(message);

        return progressCmd.ProgressId;
    }
}