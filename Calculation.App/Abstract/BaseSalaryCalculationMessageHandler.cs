using System.Text.RegularExpressions;
using AutoMapper;
using Calculation.Data;
using Calculation.Data.Entities;
using Dictionary.Models;
using Microsoft.Extensions.Logging;
using NCalc;
using Organization.Data.Data;
using Organization.Data.Enums;
using SalaryCalculation.Data;
using SalaryCalculation.Data.BaseHandlers;
using SalaryCalculation.Data.BaseModels;
using SalaryCalculation.Shared.Extensions.EnumExtensions;
using SalaryCalculation.Shared.Extensions.MoreLinq;
using Schedule.Data.Data;

namespace Calculation.App.Abstract;

public abstract class BaseSalaryCalculationMessageHandler<TMessage> : BaseMessageHandler<TMessage>
    where TMessage : BaseMessage
{
    protected new ICalculationUnitOfWork Work;
    protected readonly IOrganizationUnitOfWork OrganizationUnitOfWork;
    protected readonly IScheduleUnitOfWork ScheduleUnitOfWork;
    protected BaseLoadedData Data { get; set; }
    public BaseSalaryCalculationMessageHandler(ICalculationUnitOfWork work, ILogger<BaseSalaryCalculationMessageHandler<TMessage>> logger, IMapper mapper,
        IOrganizationUnitOfWork organizationUnitOfWork, IScheduleUnitOfWork scheduleUnitOfWork) : base(work, logger, mapper)
    {
        Work = work;
        OrganizationUnitOfWork = organizationUnitOfWork;
        ScheduleUnitOfWork = scheduleUnitOfWork;
    }

    public override async Task HandleAsync(TMessage msg)
    {
        Data = await LoadCacheDataAsync(msg);
    }

    protected abstract Task<BaseLoadedData> LoadCacheDataAsync(TMessage msg);

    protected string EvaluateConditions(IEnumerable<Formula> allowedFormulas)
    {
        if (allowedFormulas == null)
            return null;
        foreach (var formula in allowedFormulas)
        {
            var condition = ParseCondition(formula.Condition);
            if (condition)
                return formula.Expression;
        }

        return null;
    }

    protected bool ParseCondition(string condition)
    {
        if (condition == null)
            return true;
        var splitted = condition.Trim().Split("or");
        foreach (var or in splitted)
        {
            var and = or.Split("and");
            var andFlag = new List<bool>();
            foreach (var c in and)
                andFlag.Add(EvaluateCondition(c));

            if (andFlag.All(x => x))
                return true;
        }

        return false;
    }
    
    protected bool EvaluateCondition(string condition)
    {
        char[] operators = { '>', '<', '=', '!', '%' }; // Оператори порівняння
        
        var comparisonOperator = condition.FirstOrDefault(c => operators.Contains(c));
    
        if (comparisonOperator == default(char))
            return false;
        
        var parts = condition.Split(comparisonOperator);
    
        if (parts.Length != 2)
            return false;
    
        var leftOperand = parts[0].Trim();
        var rightOperand = parts[1].Trim();
    
        var leftOperandExists = Data.BaseAmounts.TryGetValue(leftOperand, out var leftValue);
        if (!leftOperandExists && !decimal.TryParse(leftOperand, out leftValue))
            return false;
        var rightOperandExists = Data.BaseAmounts.TryGetValue(rightOperand, out var rightValue);
        if (!rightOperandExists && !decimal.TryParse(rightOperand, out rightValue))
            return false;
    
        switch (comparisonOperator)
        {
            case '>':
                return leftValue > rightValue;
            case '<':
                return leftValue < rightValue;
            case '=':
                return leftValue == rightValue;
            case '!':
                return leftValue != rightValue;
            case '%':
                return leftValue % rightValue == 0;
            default:
                return false;
        }
    }


    protected decimal EvaluateExpression(string formula)
    {
        var evaluatingFormula = GetFullFormula(formula);
        var expression = new Expression(evaluatingFormula.FullFormula);
        foreach (var parameter in evaluatingFormula.Parameters)
            expression.Parameters[parameter.Key] = parameter.Value;

        return Math.Round((decimal)expression.Evaluate(), 2);
    }

    protected EvaluatingFormula GetFullFormula(string formula)
    {
        var result = new EvaluatingFormula()
        {
            BaseFormula = formula,
            FullFormula = formula
        };
        var parametersDict = new Dictionary<string, decimal>();
        var parameters = ExtractParameters(formula);

        foreach (var parameter in parameters)
        {
            if (Data.BaseAmounts.TryGetValue(parameter, out var value))
                parametersDict.TryAdd(parameter, value);
            else
            {
                var expanded = ExpandParameters(parameter);
                expanded.Parameters.ForEach(p => parametersDict.TryAdd(p.Key, p.Value));
                result.FullFormula = result.FullFormula.Replace(expanded.ExpressionParameter, expanded.FullFormula);
            }
        }

        result.Parameters = parametersDict;
        return result;
    }
    
    protected string[] ExtractParameters(string formula)
    {
        List<string> parameters = new List<string>();
        string pattern = @"\b[a-zA-Z]+\b"; // Регулярний вираз для знаходження параметрів

        MatchCollection matches = Regex.Matches(formula, pattern);

        foreach (Match match in matches)
        {
            parameters.Add(match.Value);
        }

        return parameters.ToArray();
    }

    protected ExpandedFormula ExpandParameters(string parameter)
    {
        Data.Formulas.TryGetValue(parameter, out var formulas);
        var formula = EvaluateConditions(formulas);
        if (string.IsNullOrWhiteSpace(formula))
            throw new Exception("Error finding formula");

        var result = new ExpandedFormula()
        {
            ExpressionParameter = parameter,
            FullFormula = formula
        };
        var fParameters = ExtractParameters(formula);
        var fParametersDict = new Dictionary<string, decimal>();
        foreach (var fParameter in fParameters)
        {
            if (Data.BaseAmounts.TryGetValue(fParameter, out var value))
                fParametersDict.TryAdd(fParameter, value);
            else
            {
                var expanded = ExpandParameters(fParameter);
                expanded.Parameters.ForEach(p => fParametersDict.TryAdd(p.Key, p.Value));
                result.FullFormula = result.FullFormula.Replace(expanded.ExpressionParameter, expanded.FullFormula);
            }
        }

        result.Parameters = fParametersDict;
        return result;
    }

    protected IEnumerable<BaseAmountShort> GetBaseVariables()
    {
        var benefits = EnumExtensions.ForEach<EBenefit>();
        foreach (var benefit in benefits)
        {
            var bVal = (int)benefit;
            yield return new(benefit.ToString(),
                bVal == 1 ? 1 :
                bVal > 1 && bVal <= 9 ? 1.5m :
                bVal >= 10 ? 2 : decimal.Zero);
        }
    }

    #region nested classes

    protected class BaseLoadedData
    {
        public ILookup<int, Formula> FormulaDict { get; set; }
        public ILookup<short, FinanceData> FinanceDict { get; set; }
        public Dictionary<string, decimal> BaseAmounts { get; set; }
        public Dictionary<string, Formula[]> Formulas { get; set; }
    }

    protected class BaseAmountShort
    {
        public string ExprName { get; set; }
        public decimal Value { get; set; }

        public BaseAmountShort()
        {
            
        }

        public BaseAmountShort(string name, decimal value)
        {
            ExprName = name;
            Value = value;
        }
    }
    
    protected class EvaluatingFormula
    {
        public string BaseFormula { get; set; }
        public string FullFormula { get; set; }
        public Dictionary<string, decimal> Parameters { get; set; }
    }

    protected class ExpandedFormula
    {
        public string ExpressionParameter { get; set; }
        public string FullFormula { get; set; }
        public Dictionary<string, decimal> Parameters { get; set; }
    }

    #endregion
}