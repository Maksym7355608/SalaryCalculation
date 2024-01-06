using AutoMapper;
using Microsoft.Extensions.Logging;
using MongoDB.Bson;
using MongoDB.Driver;
using Organization.App.Abstract;
using Organization.App.Commands;
using Organization.App.Commands.Messages;
using Organization.App.DtoModels;
using Organization.Data.BaseModels;
using Organization.Data.Data;
using Organization.Data.Entities;
using Organization.Data.Enums;
using Progress.App;
using SalaryCalculation.Data.BaseModels;
using SalaryCalculation.Shared.Common.Validation;
using SalaryCalculation.Shared.Extensions.MoreLinq;

namespace Organization.App.Handlers;

public class EmployeeCommandHandler : BaseOrganizationCommandHandler, IEmployeeCommandHandler
{
    private readonly IMongoCollection<Employee> _collection;
    public EmployeeCommandHandler(IOrganizationUnitOfWork work, ILogger<EmployeeCommandHandler> logger, IMapper mapper) : base(work, logger, mapper)
    {
        _collection = Work.GetCollection<Employee>();
    }

    public async Task<EmployeeDto> GetEmployeeAsync(int employeeId)
    {
        var employee = await _collection
            .Find(x => x.Id == employeeId)
            .FirstOrDefaultAsync();
        if (employee == null) throw new EntityNotFoundException(employeeId.ToString());
        return Mapper.Map<EmployeeDto>(employee);
    }

    public async Task<IEnumerable<EmployeeDto>> GetAllEmployeesAsync(int organizationId)
    {
        var employees = await _collection
            .Find(x => x.Organization.Id == organizationId)
            .ToListAsync();
        return Mapper.Map<IEnumerable<EmployeeDto>>(employees);
    }

    public async Task<IEnumerable<EmployeeDto>> SearchEmployeesAsync(EmployeeSearchCommand command)
    {
        var employees = await _collection
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
        if (_collection.Find(x => x.Organization.Id == command.OrganizationId
                                                     && x.OrganizationUnit.Id == command.OrganizationUnitId
                                                     && x.Position.Id == command.PositionId
                                                     && x.RollNumber == command.RollNumber).Any()) 
            throw new DuplicateBsonMemberMapAttributeException("Employee with the same character exist");

        var org = GetShortOrganizationAsync(command.OrganizationId);
        var unit = GetShortOrganizationUnitAsync(command.OrganizationUnitId);
        var pos = GetShortPositionAsync(command.PositionId);

        await Task.WhenAll(org, unit, pos);
        
        var employee = new Employee()
        {
            Id = await Work.NextValue<Employee, int>(),
            RollNumber = command.RollNumber,
            Name = new Person()
            {
                FirstName = command.Name,
                MiddleName = command.Fatherly,
                LastName = command.Surname,
                NameGenitive = command.NameGenitive,
                ShortName = command.ShortName,
            },
            Salaries = new []
            {
                new Salary()
                {
                    Amount = command.Salary,
                    DateFrom = command.DateFrom
                }
            },
            DateFrom = command.DateFrom,
            BankAccount = Mapper.Map<Bank>(command.BankAccount),
            Benefits = command.Benefits.Cast<int>(),
            Contacts = CreateContacts(command.Phone, command.Email, command.Telegram),
            Sex = (int)command.Sex,
            MarriedStatus = (int)command.MarriedStatus,
            Organization = org.Result,
            OrganizationUnit = unit.Result,
            Position = pos.Result,
            RegimeId = command.RegimeId,
        };
        
        await _collection.InsertOneAsync(employee);
    }

    public async Task<bool> UpdateEmployeeAsync(EmployeeUpdateCommand command)
    {
        var emp = await _collection.Find(x => x.Id == command.Id)
            .Project(x => new
            {
                x.Id,
                x.Salaries
            })
            .FirstOrDefaultAsync();
        if (emp == null) 
            throw new EntityNotFoundException(command.Id.ToString());
        var sal = emp.Salaries.ToList();
        if (command.DateSalaryUpdated.HasValue)
        {
            sal[sal.Count - 1].DateTo = command.DateSalaryUpdated.Value.AddDays(-1);
            sal.Add(new Salary()
            {
                Amount = command.Salary,
                DateFrom = command.DateSalaryUpdated.Value
            });
        }

        var unit = GetShortOrganizationUnitAsync(command.OrganizationUnitId);
        var pos = GetShortPositionAsync(command.PositionId);

        await Task.WhenAll(unit, pos);

        var update = Builders<Employee>.Update
            .Set(x => x.Name, new Person()
            {
                FirstName = command.Name,
                MiddleName = command.Fatherly,
                LastName = command.Surname,
                ShortName = command.ShortName,
                NameGenitive = command.NameGenitive
            })
            .Set(x => x.Salaries, sal)
            .Set(x => x.Contacts, CreateContacts(command.Phone, command.Email, command.Telegram))
            .Set(x => x.MarriedStatus, (int)command.MarriedStatus)
            .Set(x => x.Benefits, command.Benefits.Cast<int>())
            .Set(x => x.OrganizationUnit, unit.Result)
            .Set(x => x.Position, pos.Result)
            .Set(x => x.DateTo, command.DateTo);
            
        
        var result = await _collection.UpdateOneAsync(x => x.Id == command.Id, update);
        return result.ModifiedCount > 0;
    }

    public async Task<bool> DeleteEmployeeAsync(int employeeId)
    {
        var result = await _collection.DeleteOneAsync(x => x.Id == employeeId);
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
    
    private IEnumerable<Contact> CreateContacts(string phone, string email, string telegram)
    {
        var contacts = new List<Contact>();
        if(!string.IsNullOrWhiteSpace(phone))
            contacts.Add(new Contact(EContactType.Phone, phone));
        if(!string.IsNullOrWhiteSpace(email))
            contacts.Add(new Contact(EContactType.Email, email));
        if(!string.IsNullOrWhiteSpace(telegram))
            contacts.Add(new Contact(EContactType.TelegramUsername, telegram));
        return contacts;
    }

    private Task<IdNamePair> GetShortOrganizationAsync(int id) =>
        Work.GetCollection<Organization.Data.Entities.Organization>().Find(x => x.Id == id)
            .Project(x => new IdNamePair()
            {
                Id = x.Id,Name = x.Name
            }).FirstAsync();
    
    private Task<IdNamePair> GetShortOrganizationUnitAsync(int id) =>
        Work.GetCollection<OrganizationUnit>().Find(x => x.Id == id)
            .Project(x => new IdNamePair()
            {
                Id = x.Id,Name = x.Name
            }).FirstAsync();
    
    private Task<IdNamePair> GetShortPositionAsync(int id) =>
        Work.GetCollection<Position>().Find(x => x.Id == id)
            .Project(x => new IdNamePair()
            {
                Id = x.Id,Name = x.Name
            }).FirstAsync();
}