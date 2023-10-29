﻿using AutoMapper;
using Calculation.App.Commands;
using Calculation.Data;
using Calculation.Data.Entities;
using Dictionary.Models;
using Microsoft.Extensions.Logging;
using MongoDB.Bson;
using MongoDB.Driver;
using NCalc;
using Organization.Data.Entities;
using Organization.Data.Enums;
using SalaryCalculation.Data.BaseHandlers;
using SalaryCalculation.Data.BaseModels;
using SalaryCalculation.Shared.Extensions.EnumExtensions;
using SalaryCalculation.Shared.Extensions.MoreLinq;
using SalaryCalculation.Shared.Extensions.PeriodExtensions;
using Schedule.Data.Entities;

namespace Calculation.App.Handlers;

public class CalculationSalaryMessageHandler : BaseMessageHandler<PaymentCardCalculationMessage>
{
    protected new ICalculationUnitOfWork Work;
    public CalculationSalaryMessageHandler(ICalculationUnitOfWork work, ILogger<BaseMessageHandler<PaymentCardCalculationMessage>> logger, IMapper mapper) : base(work, logger, mapper)
    {
    }

    public override async Task HandleAsync(PaymentCardCalculationMessage msg)
    {
        var loadedData = await LoadCacheDataAsync(msg);
        var result = CalculateSalary(loadedData, msg);
    }

    private PaymentCard CalculateSalary(CalculationLoadedData loadedData, PaymentCardCalculationMessage msg)
    {
        var result = new PaymentCard();
        result.Employee = new IdNamePair(loadedData.Employee.Id, loadedData.Employee.RollNumber);
        result.OrganizationId = msg.OrganizationId;
        result.CalculationDate = DateTime.Now;
        result.CalculationPeriod = msg.Period;
        result.PayedAmount = EvaluateExpression(loadedData);
        return result;
    }

    private decimal EvaluateExpression(CalculationLoadedData loadedData)
    {
        var evaluatingFormula = GetFullFormula(loadedData);
        var expression = new Expression(evaluatingFormula.FullFormula);
        foreach (var parameter in evaluatingFormula.Parameters)
            expression.Parameters[parameter.Key] = parameter.Value;

        return (decimal)expression.Evaluate();
    }

    private EvaluatingFormula GetFullFormula(CalculationLoadedData loadedData)
    {
        var result = new EvaluatingFormula()
        {
            BaseFormula = loadedData.Formula.Expression,
            FullFormula = loadedData.Formula.Expression
        };
        var parametersDict = new Dictionary<string, decimal>();
        var parameters = loadedData.Formula.Expression.Split(@",.()+-/*^\**()\");

        foreach (var parameter in parameters)
        {
            if (loadedData.BaseAmounts.TryGetValue(parameter, out var value))
                parametersDict.TryAdd(parameter, value);
            else
            {
                var expanded = ExpandParameters(parameter, loadedData);
                expanded.Parameters.ForEach(p => parametersDict.TryAdd(p.Key, p.Value));
                result.FullFormula = result.FullFormula.Replace(expanded.ExpressionParameter, expanded.FullFormula);
            }
        }

        result.Parameters = parametersDict;
        return result;
    }

    private ExpandedFormula ExpandParameters(string parameter, CalculationLoadedData loadedData)
    {
        var result = new ExpandedFormula()
        {
            ExpressionParameter = parameter,
            FullFormula = loadedData.Formula.Expression
        };
        loadedData.Formulas.TryGetValue(parameter, out var formula);
        if (string.IsNullOrWhiteSpace(formula))
            throw new Exception("Error finding formula");
        var fParameters = formula.Split(@",.()+-/*^\**()\"); //TODO: do worked regex
        var fParametersDict = new Dictionary<string, decimal>();
        foreach (var fParameter in fParameters)
        {
            if (loadedData.BaseAmounts.TryGetValue(fParameter, out var value))
                fParametersDict.TryAdd(fParameter, value);
            else
            {
                var expanded = ExpandParameters(fParameter, loadedData);
                expanded.Parameters.ForEach(p => fParametersDict.TryAdd(p.Key, p.Value));
                result.FullFormula = result.FullFormula.Replace(expanded.ExpressionParameter, expanded.FullFormula);
            }
        }

        result.Parameters = fParametersDict;
        return result;
    }

    #region LoadedData

    private async Task<CalculationLoadedData> LoadCacheDataAsync(PaymentCardCalculationMessage msg)
    {
        var formulaTask = LoadFormulaAsync(msg.FormulaId);
        var employeeTask = LoadEmployeeDataAsync(msg.EmployeeId);
        var periodCalendarTask = LoadPeriodCalendarAsync(msg.EmployeeId, msg.Period);
        var baseAmountTask = LoadBaseAmountsAsync();
        await Task.WhenAll(formulaTask, employeeTask, periodCalendarTask, baseAmountTask);
        var regime = await LoadRegimeAsync(periodCalendarTask.Result.RegimeId);
        return new CalculationLoadedData()
        {
            Formula = formulaTask.Result,
            Employee = employeeTask.Result,
            Calendar = periodCalendarTask.Result,
            BaseAmounts = baseAmountTask.Result.Concat(GetVariables(employeeTask.Result, periodCalendarTask.Result))
                .ToDictionary(k => k.ExprName, v => v.Value),
            Regime = regime
        };
    }

    private List<BaseAmountShort> GetVariables(Employee employee, PeriodCalendar calendar)
    {
        var currentSalary = employee.Salaries.FirstOrDefault(x =>
            x.DateFrom <= calendar.Period.ToDateTime() &&
            (!x.DateTo.HasValue || x.DateTo.Value > calendar.Period.ToDateTime().AddMonths(1)))?.Amount ?? decimal.Zero;
        var paramsDict = new List<BaseAmountShort>()
        {
            new("Salary", currentSalary),
            new("SummaryHours", calendar.Hours.Summary),
            new("DayHours", calendar.Hours.Day),
            new("EveningHours", calendar.Hours.Evening),
            new("NightHours", calendar.Hours.Night),
            new("HolidayHours", calendar.Hours.Holiday),
            new(nameof(calendar.WorkDays), calendar.WorkDays),
            new(nameof(calendar.SickLeave), calendar.SickLeave),
            new(nameof(calendar.VacationDays), calendar.VacationDays)
        };
        var benefits = EnumExtensions.ForEach<EBenefit>();
        foreach (var benefit in benefits)
        {
            var bVal = (int)benefit;
            paramsDict.Add(new(benefit.ToString(),
                bVal == 1 ? 1 : 
                bVal > 1 && bVal <= 9 ? 1.5m : 
                bVal >= 10 ? 2 : decimal.Zero));
        }
        return paramsDict;
    }

    private Task<List<BaseAmountShort>> LoadBaseAmountsAsync()
    {
        return Work.GetCollection<BaseAmount>()
            .Find(Builders<BaseAmount>.Filter.Empty)
            .Project(x => new BaseAmountShort(x.ExpressionName, x.Value))
            .ToListAsync();
    }

    private Task<Regime> LoadRegimeAsync(int resultRegimeId)
    {
        throw new NotImplementedException();
    }

    private Task<PeriodCalendar> LoadPeriodCalendarAsync(int employeeId, int period)
    {
        throw new NotImplementedException();
    }

    private Task<Employee> LoadEmployeeDataAsync(int employeeId)
    {
        throw new NotImplementedException();
    }

    private Task<Formula> LoadFormulaAsync(ObjectId formulaId)
    {
        throw new NotImplementedException();
    }


    private class CalculationLoadedData
    {
        public Formula Formula { get; set; }
        public Employee Employee { get; set; }
        public PeriodCalendar Calendar { get; set; }
        public Regime Regime { get; set; }
        public Dictionary<string, string> Formulas { get; set; }
        public Dictionary<string, decimal> BaseAmounts { get; set; }
    }

    private class BaseAmountShort
    {
        public string ExprName { get; set; }
        public decimal Value { get; set; }

        public BaseAmountShort(string name, decimal value)
        {
            ExprName = name;
            Value = value;
        }
    }

    #endregion

    #region nested Classes

    private class EvaluatingFormula
    {
        public string BaseFormula { get; set; }
        public string FullFormula { get; set; }
        public Dictionary<string, decimal> Parameters { get; set; }
    }

    private class ExpandedFormula
    {
        public string ExpressionParameter { get; set; }
        public string FullFormula { get; set; }
        public Dictionary<string, decimal> Parameters { get; set; }
    }

    #endregion
}