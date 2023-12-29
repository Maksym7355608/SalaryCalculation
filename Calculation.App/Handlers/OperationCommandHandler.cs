using AutoMapper;
using Calculation.App.Abstract;
using Calculation.App.Commands;
using Calculation.App.DtoModels;
using Calculation.Data;
using Calculation.Data.Entities;
using Microsoft.Extensions.Logging;
using MongoDB.Bson;
using MongoDB.Driver;
using SalaryCalculation.Data.BaseHandlers;
using SalaryCalculation.Shared.Common.Validation;
using SalaryCalculation.Shared.Extensions.PeriodExtensions;

namespace Calculation.App.Handlers;

public class OperationCommandHandler : BaseCalculationCommandHandler, IOperationCommandHandler
{
    public OperationCommandHandler(ICalculationUnitOfWork work, ILogger<BaseCommandHandler> logger, IMapper mapper) : base(work, logger, mapper)
    {
    }


    public async Task<OperationDto> GetOperationAsync(long id)
    {
        var operation = await Work.GetCollection<Operation>()
            .Find(x => x.Id == id)
            .FirstOrDefaultAsync();
        if (operation == null)
            throw new EntityNotFoundException(id.ToString());
        return Mapper.Map<OperationDto>(operation);
    }

    public async Task<IEnumerable<OperationDto>> GetOperationsByEmployeeAsync(int employeeId, int? period)
    {
        var operations = await Work.GetCollection<Operation>()
            .Find(x => x.EmployeeId == employeeId && (!period.HasValue || period == x.Period))
            .ToListAsync();
        if (operations == null || operations.Count == 0)
            throw new EntityNotFoundException(
                "Operations with employee id: {0} " + (period.HasValue ? "and period {1} " : string.Empty) +
                "was not found", employeeId.ToString(), period?.ToShortPeriodString() ?? string.Empty);
        return Mapper.Map<IEnumerable<OperationDto>>(operations);
    }

    public async Task<IEnumerable<OperationDto>> SearchOperationsAsync(OperationsSearchCommand command)
    {
        var filterBuilder = Builders<Operation>.Filter;
        var filter = filterBuilder.Empty;

        if (command.Code.HasValue)
        {
            filter = filter & filterBuilder.Eq(x => x.Code, command.Code);
        }

        if (!string.IsNullOrEmpty(command.Name))
        {
            filter = filter & filterBuilder.Regex(x => x.Name, new BsonRegularExpression(command.Name, "i"));
        }

        if (command.Period.HasValue)
        {
            filter = filter & filterBuilder.Eq(x => x.Period, command.Period);
        }

        if (command.AmountFrom.HasValue)
        {
            filter = filter & filterBuilder.Gte(x => x.Amount, command.AmountFrom);
        }

        if (command.AmountTo.HasValue)
        {
            filter = filter & filterBuilder.Lte(x => x.Amount, command.AmountTo);
        }

        filter = filter & filterBuilder.Eq(x => x.OrganizationId, command.OrganizationId);

        var operations = await Work.GetCollection<Operation>().Find(filter).ToListAsync();
        return Mapper.Map<IEnumerable<OperationDto>>(operations);
    }

    public async Task AddOperationAsync(OperationCreateCommand command)
    {
        var filterBuilder = Builders<Operation>.Filter;
        var filter = filterBuilder.And(
            filterBuilder.Eq(x => x.Code, command.Code),
            filterBuilder.Eq(x => x.EmployeeId, command.EmployeeId),
            filterBuilder.Eq(x => x.Period, command.Period)
        );
        
        var existingOperation = await Work.GetCollection<Operation>().Find(filter).FirstOrDefaultAsync();

        if (existingOperation != null)
        {
            throw new InvalidOperationException("Operation exist");
        }
    
        var operation = Mapper.Map<Operation>(command);
        await Work.GetCollection<Operation>().InsertOneAsync(operation);
    }

    public async Task<bool> UpdateOperationAsync(OperationUpdateCommand command)
    {
        var operation = Mapper.Map<Operation>(command);
        var result = await Work.GetCollection<Operation>().ReplaceOneAsync(x => x.Id == command.Id, operation);

        if (result.ModifiedCount == 0)
        {
            throw new InvalidOperationException("Operation does not exist");
        }

        return true;
    }

    public async Task<bool> DeleteOperationAsync(long id)
    {
        var result = await Work.GetCollection<Operation>().DeleteOneAsync(x => x.Id == id);
        if(result.DeletedCount == 0)
            throw new InvalidOperationException("Operation was no delete");
        return true;
    }
}