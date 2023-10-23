using AutoMapper;
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
        var parameters = loadedData.Formula.Expression.Split(@",.()+-/*^\**()\");
        var expression = new Expression(loadedData.Formula.Expression);
        foreach (var parameterName in parameters)
        {
            if (loadedData.BaseAmounts.TryGetValue(parameterName, out var value))
                expression.Parameters[parameterName] = value;
            else
            {
                //TODO: Do for hard expressions
            }
        }

        return (decimal)expression.Evaluate();
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
    
}