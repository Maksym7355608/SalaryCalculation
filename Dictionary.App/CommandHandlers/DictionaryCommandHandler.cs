using System.Data;
using AutoMapper;
using Dictionary.App.Abstract;
using Dictionary.App.Commands;
using Dictionary.App.Dto;
using Dictionary.Models;
using Microsoft.Extensions.Logging;
using MongoDB.Bson;
using MongoDB.Driver;
using Organization.Data.Enums;
using SalaryCalculation.Data;
using SalaryCalculation.Data.BaseHandlers;
using SalaryCalculation.Shared.Common.Validation;
using SalaryCalculation.Shared.Extensions.EnumExtensions;

namespace Dictionary.App.CommandHandlers;

public class DictionaryCommandHandler : BaseCommandHandler, IDictionaryCommandHandler
{
    private readonly string[] _baseExpressionNames = new[]
    {
        "S", "AvgS", "SumH", "DH", "EH", "NH", "HolH", "HolDH", "HolEH", "HolNH", "WD", "SickL",
        "VacL", "Period", "TotH", "TotDH", "TotEH", "TotNH", "TotHolH", "TotHolDH", "TotHolEH",
        "TotHolNH"
    }.Concat(EnumExtensions.ForEach<EBenefit>().Select(x => x.ToString())).ToArray();

    private readonly string[] _operators = new[]
    {
        ">", "<", "=", "!", "and", "or", "%", "+", "-", "*", "/", "(", ")", "sqrt", "^"
    };
    public DictionaryCommandHandler(IUnitOfWork work, ILogger<BaseCommandHandler> logger, IMapper mapper) : base(work, logger, mapper)
    {
    }


    public async Task<List<BaseAmountDto>> SearchBaseAmounts(BaseAmountsSearchCommand command)
    {
        var filter = GetBaseAmountSearchFilter(command);

        var result = await Work.GetCollection<BaseAmount>()
            .Find(filter)
            .ToListAsync();

        return Mapper.Map<List<BaseAmountDto>>(result);
    }

    private FilterDefinition<BaseAmount> GetBaseAmountSearchFilter(BaseAmountsSearchCommand command)
    {
        var builder = Builders<BaseAmount>.Filter;
        var definition = new List<FilterDefinition<BaseAmount>>();

        if(!string.IsNullOrWhiteSpace(command.Name))
            definition.Add(builder.Regex(x => x.Name, new BsonRegularExpression(command.Name, "i")));
        if(!string.IsNullOrWhiteSpace(command.ExpressionName))
            definition.Add(builder.Regex(x => x.ExpressionName, new BsonRegularExpression(command.ExpressionName, "i")));
        if (command.DateFrom.HasValue)
            definition.AddRange(new[]
            {
                builder.Gte(x => x.DateFrom, command.DateFrom.Value),
                builder.Or(builder.Gte(x => x.DateTo, command.DateFrom.Value),
                    builder.Eq(x => x.DateTo, null))
            });
        if (command.DateTo.HasValue)
            definition.AddRange(new[]
            {
                builder.Lte(x => x.DateFrom, command.DateTo.Value),
                builder.Or(builder.Lte(x => x.DateTo, command.DateTo.Value),
                    builder.Eq(x => x.DateTo, null))
            });
        
        return definition.Count > 0 ? builder.And(definition) : builder.Empty;
    }

    public async Task<List<FinanceDataDto>> SearchFinanceData(FinanceDataSearchCommand command)
    {
        var filter = GetFinanceDataSearchFilter(command);

        var result = await Work.GetCollection<FinanceData>()
            .Find(filter)
            .ToListAsync();

        return Mapper.Map<List<FinanceDataDto>>(result);
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
        if(command.Codes != null && command.Codes.Any())
            definition.Add(builder.In(x => x.Code, command.Codes));
        
        
        return builder.And(definition);
    }

    public async Task<List<FormulaDto>> SearchFormulas(FormulasSearchCommand command)
    {
        var filter = GetFormulaSearchFilter(command);

        var result = await Work.GetCollection<Formula>()
            .Find(filter)
            .ToListAsync();

        return Mapper.Map<List<FormulaDto>>(result);
    }
    
    private FilterDefinition<Formula> GetFormulaSearchFilter(FormulasSearchCommand command)
    {
        var builder = Builders<Formula>.Filter;
        if (!string.IsNullOrWhiteSpace(command.Id))
            return builder.Eq(x => x.Id, new ObjectId(command.Id));
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
        var formulas = await Work.GetCollection<Formula>()
            .Find(x => x.OrganizationId == command.OrganizationId)
            .Project(x => x.ExpressionName)
            .ToListAsync();
        var baseAmounts = await Work.GetCollection<BaseAmount>()
            .Find(a => true)
            .Project(x => x.ExpressionName)
            .ToListAsync();
        IsConditionValid(command.Condition, baseAmounts);
        IsExpressionValid(command.Expression, formulas, baseAmounts);

        var item = Mapper.Map<Formula>(command);
        await Work.GetCollection<Formula>().InsertOneAsync(item);
        return true;
    }

    private void IsConditionValid(string? condition, List<string> baseAmounts)
    {
        if(string.IsNullOrWhiteSpace(condition))
            return;
        var split = condition.Trim().Split(_operators, StringSplitOptions.None);
        var incorrectValues = split.Where(x => !_baseExpressionNames.Contains(x) && !x.All(c => char.IsDigit(c) || c == '.')
        && !baseAmounts.Contains(x));
        if (incorrectValues.Any())
            throw new Exception($"Incorrect condition members: {string.Join(", ", incorrectValues)}");
    }
    
    private void IsExpressionValid(string expression, List<string> formulas, List<string> baseAmounts)
    {
        if(string.IsNullOrWhiteSpace(expression))
            throw new Exception("Expression is empty");
        var split = expression.Trim().Split(_operators, StringSplitOptions.None);
        var incorrectValues = split.Where(x => !_baseExpressionNames.Contains(x) && !x.All(c => char.IsDigit(c) || c == '.')
            && !formulas.Contains(x) && !baseAmounts.Contains(x));
        if (incorrectValues.Any())
            throw new Exception($"Incorrect expression members: {string.Join(", ", incorrectValues)}");
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