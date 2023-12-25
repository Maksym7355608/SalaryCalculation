using AutoMapper;
using Calculation.App.Commands;
using Calculation.Data;
using Calculation.Data.Entities;
using Dictionary.Models;
using Microsoft.Extensions.Logging;
using MongoDB.Driver;
using NCalc;
using Organization.Data.Data;
using Organization.Data.Entities;
using Organization.Data.Enums;
using SalaryCalculation.Data.BaseHandlers;
using SalaryCalculation.Data.BaseModels;
using SalaryCalculation.Shared.Extensions.EnumExtensions;
using SalaryCalculation.Shared.Extensions.MoreLinq;
using SalaryCalculation.Shared.Extensions.PeriodExtensions;
using Schedule.Data.Data;
using Schedule.Data.Entities;

namespace Calculation.App.Handlers;

public class CalculationSalaryMessageHandler : BaseMessageHandler<CalculationSalaryMessage>
{
    protected new ICalculationUnitOfWork Work;
    private readonly IOrganizationUnitOfWork _organizationUnitOfWork;
    private readonly IScheduleUnitOfWork _scheduleUnitOfWork;
    public CalculationSalaryMessageHandler(ICalculationUnitOfWork work, ILogger<BaseMessageHandler<CalculationSalaryMessage>> logger, IMapper mapper,
        IOrganizationUnitOfWork organizationUnitOfWork, IScheduleUnitOfWork scheduleUnitOfWork) : base(work, logger, mapper)
    {
        _organizationUnitOfWork = organizationUnitOfWork;
        _scheduleUnitOfWork = scheduleUnitOfWork;
    }

    public override async Task HandleAsync(CalculationSalaryMessage msg)
    {
        var loadedData = await LoadCacheDataAsync(msg);
        var result = CalculateSalary(loadedData, msg);
    }

    private PaymentCard CalculateSalary(CalculationLoadedData loadedData, CalculationSalaryMessage msg)
    {
        var result = new PaymentCard();
        result.Employee = new IdNamePair(loadedData.Employee.Id, loadedData.Employee.RollNumber);
        result.OrganizationId = msg.OrganizationId;
        result.CalculationDate = DateTime.Now;
        result.CalculationPeriod = msg.Period;
        result.PayedAmount = EvaluateExpression(loadedData); //TODO: доробка логіки для деталізації
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

    private async Task<CalculationLoadedData> LoadCacheDataAsync(CalculationSalaryMessage msg)
    {
        var formulaTask = LoadFormulasAsync(msg.OrganizationId);
        var employeeTask = LoadEmployeeDataAsync(msg.EmployeeId);
        var periodCalendarTask = LoadPeriodCalendarAsync(msg.EmployeeId, msg.Period);
        var baseAmountTask = LoadBaseAmountsAsync();
        await Task.WhenAll(formulaTask, employeeTask, periodCalendarTask, baseAmountTask);
        var regime = await LoadRegimeAsync(periodCalendarTask.Result.RegimeId);
        return new CalculationLoadedData()
        {
            Formula = formulaTask.Result.First(x => x.Id == msg.FormulaId),
            Employee = employeeTask.Result,
            Calendar = periodCalendarTask.Result,
            BaseAmounts = baseAmountTask.Result.Concat(GetVariables(employeeTask.Result, periodCalendarTask.Result))
                .ToDictionary(k => k.ExprName, v => v.Value),
            Regime = regime,
            Formulas = formulaTask.Result.ToDictionary(k => k.ExpressionName, v => v.Expression)
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
            new("HolidayHours", calendar.Hours.HolidaySummary),
            new("HolidayDay", calendar.Hours.HolidayDay),
            new("HolidayEvening", calendar.Hours.HolidayEvening),
            new("HolidayNight", calendar.Hours.HolidayNight),
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

    private Task<Regime> LoadRegimeAsync(int regimeId)
    {
        return _scheduleUnitOfWork.GetCollection<Regime>()
            .Find(x => x.Id == regimeId)
            .FirstOrDefaultAsync();
    }

    private Task<PeriodCalendar> LoadPeriodCalendarAsync(int employeeId, int period)
    {
        return _scheduleUnitOfWork.GetCollection<PeriodCalendar>()
            .Find(x => x.EmployeeId == employeeId && x.Period == period)
            .FirstOrDefaultAsync();
    }

    private Task<Employee> LoadEmployeeDataAsync(int employeeId)
    {
        return _organizationUnitOfWork.GetCollection<Employee>()
            .Find(x => x.Id == employeeId)
            .FirstOrDefaultAsync();
    }

    private Task<List<Formula>> LoadFormulasAsync(int organizationId)
    {
        return Work.GetCollection<Formula>()
            .Find(x => x.OrganizationId == organizationId)
            .ToListAsync();
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