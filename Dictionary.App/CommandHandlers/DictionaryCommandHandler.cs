using System.Data;
using AutoMapper;
using Dictionary.App.Abstract;
using Dictionary.App.Commands;
using Dictionary.Models;
using Microsoft.Extensions.Logging;
using MongoDB.Bson;
using MongoDB.Driver;
using SalaryCalculation.Data;
using SalaryCalculation.Data.BaseHandlers;
using SalaryCalculation.Shared.Common.Validation;

namespace Dictionary.App.CommandHandlers;

public class DictionaryCommandHandler : BaseCommandHandler, IDictionaryCommandHandler
{
    public DictionaryCommandHandler(IUnitOfWork work, ILogger<BaseCommandHandler> logger, IMapper mapper) : base(work, logger, mapper)
    {
    }


    public Task<List<BaseAmount>> SearchBaseAmounts(BaseAmountsSearchCommand command)
    {
        var filter = GetBaseAmountSearchFilter(command);

        var result = Work.GetCollection<BaseAmount>()
            .Find(filter)
            .ToListAsync();

        return result;
    }

    private FilterDefinition<BaseAmount> GetBaseAmountSearchFilter(BaseAmountsSearchCommand command)
    {
        var builder = Builders<BaseAmount>.Filter;
        var definition = new List<FilterDefinition<BaseAmount>>();

        if(!string.IsNullOrWhiteSpace(command.Name))
            definition.Add(builder.Regex(x => x.Name, new BsonRegularExpression(command.Name, "i")));
        if(!string.IsNullOrWhiteSpace(command.ExpressionName))
            definition.Add(builder.Regex(x => x.ExpressionName, new BsonRegularExpression(command.ExpressionName, "i")));
        
        return definition.Count > 0 ? builder.And(definition) : builder.Empty;
    }

    public Task<List<FinanceData>> SearchFinanceData(FinanceDataSearchCommand command)
    {
        var filter = GetFinanceDataSearchFilter(command);

        var result = Work.GetCollection<FinanceData>()
            .Find(filter)
            .ToListAsync();

        return result;
    }
    
    private FilterDefinition<FinanceData> GetFinanceDataSearchFilter(FinanceDataSearchCommand command)
    {
        var builder = Builders<FinanceData>.Filter;
        var definition = new List<FilterDefinition<FinanceData>>()
        {
            builder.Eq(x => x.OrganizationId, command.OrganizationId),
        };

        if(!string.IsNullOrWhiteSpace(command.Name))
            definition.Add(builder.Regex(x => x.Name, new BsonRegularExpression(command.Name, "i")));
        if(command.Codes.Any())
            definition.Add(builder.In(x => x.Code, command.Codes));
        
        
        return builder.And(definition);
    }

    public Task<List<Formula>> SearchFormulas(FormulasSearchCommand command)
    {
        var filter = GetFormulaSearchFilter(command);

        var result = Work.GetCollection<Formula>()
            .Find(filter)
            .ToListAsync();

        return result;
    }
    
    private FilterDefinition<Formula> GetFormulaSearchFilter(FormulasSearchCommand command)
    {
        var builder = Builders<Formula>.Filter;
        var definition = new List<FilterDefinition<Formula>>()
        {
            builder.Eq(x => x.OrganizationId, command.OrganizationId),
            builder.Gte(x => x.DateFrom, command.DateFrom)
        };

        if(!string.IsNullOrWhiteSpace(command.Name))
            definition.Add(builder.Regex(x => x.Name, new BsonRegularExpression(command.Name, "i")));
        if(!string.IsNullOrWhiteSpace(command.ExpressionName))
            definition.Add(builder.Regex(x => x.ExpressionName, new BsonRegularExpression(command.ExpressionName, "i")));
        if(command.DateTo.HasValue)
            definition.Add(builder.Or(builder.Eq(x => x.DateTo, null), builder.Lte(x => x.DateTo, command.DateTo.Value)));
        
        
        return builder.And(definition);
    }

    public async Task<bool> CreateBaseAmount(BaseAmountCreateCommand command)
    {
        var existingItem = await Work.GetCollection<BaseAmount>()
            .Find(x => (x.Name == command.Name || x.ExpressionName == command.ExpressionName)
                && (!x.DateTo.HasValue || x.DateTo > command.DateFrom))
            .AnyAsync();
        if (existingItem)
            throw new DuplicateNameException();

        var item = Mapper.Map<BaseAmount>(command);
        await Work.GetCollection<BaseAmount>().InsertOneAsync(item);
        return true;
    }

    public async Task<bool> CreateFinanceData(FinanceDataCreateCommand command)
    {
        var existingItem = await Work.GetCollection<FinanceData>()
            .Find(x => x.Code == command.Code)
            .AnyAsync();
        if (existingItem)
            throw new DuplicateNameException();

        var item = Mapper.Map<FinanceData>(command);
        await Work.GetCollection<FinanceData>().InsertOneAsync(item);
        return true;
    }

    public async Task<bool> CreateFormula(FormulaCreateCommand command)
    {
        var existingItem = await Work.GetCollection<Formula>()
            .Find(x => x.ExpressionName == command.ExpressionName && x.Condition == command.Condition 
                                 && (!x.DateTo.HasValue || x.DateTo < command.DateFrom))
            .AnyAsync();
        if (existingItem)
            throw new DuplicateNameException();

        var item = Mapper.Map<Formula>(command);
        await Work.GetCollection<Formula>().InsertOneAsync(item);
        return true;
    }

    public async Task<bool> UpdateBaseAmount(ObjectId id, BaseAmountCreateCommand command)
    {
        var existingItem = await Work.GetCollection<BaseAmount>()
            .Find(x => x.Id == id)
            .AnyAsync();
        if (!existingItem)
            throw new EntityNotFoundException(id.ToString());

        var item = Mapper.Map<BaseAmount>(command);
        item.Id = id;
        var res = await Work.GetCollection<BaseAmount>().ReplaceOneAsync(x => x.Id == id, item);
        return res.ModifiedCount > 0;
    }

    public async Task<bool> UpdateFinanceData(ObjectId id, FinanceDataCreateCommand command)
    {
        var existingItem = await Work.GetCollection<FinanceData>()
            .Find(x => x.Id == id)
            .AnyAsync();
        if (!existingItem)
            throw new EntityNotFoundException(id.ToString());

        var item = Mapper.Map<FinanceData>(command);
        item.Id = id;
        var res = await Work.GetCollection<FinanceData>().ReplaceOneAsync(x => x.Id == id, item);
        return res.ModifiedCount > 0;
    }

    public async Task<bool> UpdateFormula(ObjectId id, FormulaCreateCommand command)
    {
        var existingItem = await Work.GetCollection<Formula>()
            .Find(x => x.Id == id)
            .AnyAsync();
        if (!existingItem)
            throw new EntityNotFoundException(id.ToString());

        var item = Mapper.Map<Formula>(command);
        item.Id = id;
        var res = await Work.GetCollection<Formula>().ReplaceOneAsync(x => x.Id == id, item);
        return res.ModifiedCount > 0;
    }

    public async Task<bool> DeleteBaseAmount(ObjectId id)
    {
        var result = await Work.GetCollection<BaseAmount>()
            .DeleteOneAsync(x => x.Id == id);
        return result.DeletedCount > 0;
    }

    public async Task<bool> DeleteFinanceData(ObjectId id)
    {
        var result = await Work.GetCollection<FinanceData>()
            .DeleteOneAsync(x => x.Id == id);
        return result.DeletedCount > 0;
    }

    public async Task<bool> DeleteFormula(ObjectId id)
    {
        var result = await Work.GetCollection<Formula>()
            .DeleteOneAsync(x => x.Id == id);
        return result.DeletedCount > 0;
    }
}