using System.Text.RegularExpressions;
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
using Schedule.App.Helpers;
using Schedule.App.Models;
using Schedule.Data.BaseModels;
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
        Work = work;
        _organizationUnitOfWork = organizationUnitOfWork;
        _scheduleUnitOfWork = scheduleUnitOfWork;
    }

    public override async Task HandleAsync(CalculationSalaryMessage msg)
    {
        var loadedData = await LoadCacheDataAsync(msg);
        var result = await CalculateSalaryAsync(loadedData, msg);
        await Work.GetCollection<PaymentCard>().InsertOneAsync(result);
    }

    private async Task<PaymentCard> CalculateSalaryAsync(CalculationLoadedData loadedData, CalculationSalaryMessage msg)
    {
        var result = new PaymentCard();
        result.Employee = new IdNamePair(loadedData.Employee.Id, loadedData.Employee.Name.ShortName);
        result.OrganizationId = msg.OrganizationId;
        result.CalculationDate = DateTime.Now;
        result.CalculationPeriod = msg.Period;
        
        //var accrualOperationsTask = CreateOperationsAsync(loadedData.FinanceDict[1], loadedData, 1);
        //var maintenanceOperationsTask = CreateOperationsAsync(loadedData.FinanceDict[-1], loadedData, -1);
        //await Task.WhenAll(accrualOperationsTask, maintenanceOperationsTask);
        var accrualOperations = await CreateOperationsAsync(loadedData.FinanceDict[1], loadedData, 1);
        var maintenanceOperations = await CreateOperationsAsync(loadedData.FinanceDict[-1], loadedData, -1);
        var operations = await InsertOperationsAsync(accrualOperations, maintenanceOperations, msg.EmployeeId, msg.Period);
        result.AccrualAmount = accrualOperations.Sum(x => x.Amount);
        result.MaintenanceAmount = maintenanceOperations.Sum(x => x.Amount);
        result.PayedAmount = result.AccrualAmount - result.MaintenanceAmount;
        result.AccrualDetails = operations.Where(x => x.Sign == 1).Select(x => x.Id);
        result.MaintenanceDetails = operations.Where(x => x.Sign == -1).Select(x => x.Id);
        result.Id = await Work.NextValue<PaymentCard, int>();
        return result;
    }

    private async Task<List<Operation>> InsertOperationsAsync(List<Operation> accrualOperations, List<Operation> maintenanceOperations,
        int employeeId, int period)
    {
        await Work.GetCollection<Operation>().DeleteManyAsync(x => x.EmployeeId == employeeId && x.Period == period);
        var operations = new List<Operation>();
        operations.AddRange(accrualOperations);
        operations.AddRange(maintenanceOperations);
        var lastId = await Work.NextValue<Operation, long>(operations.Count);
        
        operations.ForEach(op => op.Id = lastId--);
        await Work.GetCollection<Operation>().InsertManyAsync(operations);
        return operations;
    }

    private async Task<List<Operation>> CreateOperationsAsync(IEnumerable<FinanceData> data,
        CalculationLoadedData loadedData, int sign)
    {
        var ops = new List<Operation>();
        foreach (var fin in data)
        {
            var allowedFormulas = loadedData.FormulaDict[fin.Code];
            var formula = EvaluateConditions(allowedFormulas, loadedData);
            if(formula == null)
                continue;
            var amount = EvaluateExpression(formula, loadedData);
            var operation = new Operation()
            {
                Code = fin.Code,
                Name = fin.Name,
                EmployeeId = loadedData.Employee.Id,
                Amount = amount,
                Sign = sign,
                Period = loadedData.Calendar.Period,
                OrganizationId = loadedData.Employee.Organization.Id
            };
            ops.Add(operation);
        }

        return ops;
    }

    private string EvaluateConditions(IEnumerable<Formula> allowedFormulas, CalculationLoadedData loadedData)
    {
        if (allowedFormulas == null)
            return null;
        foreach (var formula in allowedFormulas)
        {
            var condition = ParseCondition(formula.Condition, loadedData);
            if (condition)
                return formula.Expression;
        }

        return null;
    }

    private bool ParseCondition(string condition, CalculationLoadedData data)
    {
        if (condition == null)
            return true;
        var splitted = condition.Trim().Split("or");
        foreach (var or in splitted)
        {
            var and = or.Split("and");
            var andFlag = new List<bool>();
            foreach (var c in and)
                andFlag.Add(EvaluateCondition(c, data.BaseAmounts));

            if (andFlag.All(x => x))
                return true;
        }

        return false;
    }
    
    public bool EvaluateCondition(string condition, Dictionary<string, decimal> values)
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
    
        var leftOperandExists = values.TryGetValue(leftOperand, out var leftValue);
        if (!leftOperandExists && !decimal.TryParse(leftOperand, out leftValue))
            return false;
        var rightOperandExists = values.TryGetValue(rightOperand, out var rightValue);
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


    private decimal EvaluateExpression(string formula, CalculationLoadedData data)
    {
        var evaluatingFormula = GetFullFormula(formula, data);
        var expression = new Expression(evaluatingFormula.FullFormula);
        foreach (var parameter in evaluatingFormula.Parameters)
            expression.Parameters[parameter.Key] = parameter.Value;

        return Math.Round((decimal)expression.Evaluate(), 2);
    }

    private EvaluatingFormula GetFullFormula(string formula, CalculationLoadedData data)
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
            if (data.BaseAmounts.TryGetValue(parameter, out var value))
                parametersDict.TryAdd(parameter, value);
            else
            {
                var expanded = ExpandParameters(parameter, data);
                expanded.Parameters.ForEach(p => parametersDict.TryAdd(p.Key, p.Value));
                result.FullFormula = result.FullFormula.Replace(expanded.ExpressionParameter, expanded.FullFormula);
            }
        }

        result.Parameters = parametersDict;
        return result;
    }
    
    private string[] ExtractParameters(string formula)
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

    private ExpandedFormula ExpandParameters(string parameter, CalculationLoadedData data)
    {
        data.Formulas.TryGetValue(parameter, out var formulas);
        var formula = EvaluateConditions(formulas, data);
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
            if (data.BaseAmounts.TryGetValue(fParameter, out var value))
                fParametersDict.TryAdd(fParameter, value);
            else
            {
                var expanded = ExpandParameters(fParameter, data);
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
        var financeData = await LoadFinanceDataAsync(msg.OrganizationId);
        var formulaTask = LoadFormulasAsync(msg.OrganizationId, financeData.Select(x => x.Code));
        var employeeTask = LoadEmployeeDataAsync(msg.EmployeeId);
        var periodCalendarTask = LoadPeriodCalendarAsync(msg.EmployeeId, msg.Period);
        var baseAmountTask = LoadBaseAmountsAsync(msg.Period);
        await Task.WhenAll(formulaTask, employeeTask, periodCalendarTask, baseAmountTask);
        var regime = await LoadRegimeAsync(periodCalendarTask.Result.RegimeId);
        var avgSalary = await LoadAverageDaySalaryAsync(employeeTask.Result.Id, msg.Period);
        var hours = await RegimeHelper.GetHoursDetailAsync(regime, periodCalendarTask.Result.Period);
        return new CalculationLoadedData()
        {
            FinanceDict = financeData.ToLookup(k => k.Sign, v => v),
            FormulaDict = formulaTask.Result.ToLookup(k => k.Code, v => v),
            Employee = employeeTask.Result,
            Calendar = periodCalendarTask.Result,
            BaseAmounts = baseAmountTask.Result.Concat(GetVariables(employeeTask.Result, periodCalendarTask.Result, hours[employeeTask.Result.ShiftNumber], avgSalary))
                .ToDictionary(k => k.ExprName, v => v.Value),
            Regime = regime,
            Formulas = formulaTask.Result.GroupBy(k => k.ExpressionName).ToDictionary(k => k.Key, v=> v.ToArray()),
        };
    }

    private async Task<decimal> LoadAverageDaySalaryAsync(int id, int period)
    {
        var periodFrom = period / 100 * 100 + 1;
        var periodTo = period.PreviousPeriod();
        var calendarsTask = _scheduleUnitOfWork.GetCollection<PeriodCalendar>()
            .Find(x => x.EmployeeId == id && x.Period >= periodFrom && x.Period <= periodTo)
            .Project(x => x.WorkDays)
            .ToListAsync();
        var paymentsTask = Work.GetCollection<PaymentCard>()
            .Find(x => x.Employee.Id == id && x.CalculationPeriod >= periodFrom && x.CalculationPeriod <= periodTo)
            .Project(x => x.AccrualAmount)
            .ToListAsync();
        await Task.WhenAll(calendarsTask, paymentsTask);
        var calendar = calendarsTask.Result;
        var payment = paymentsTask.Result;
        if (!calendar.Any())
            return 0;
        if (!payment.Any())
        {
            return 0;
        }

        return (payment.Sum()) / (calendar.Sum());
    }

    private Task<List<FinanceData>> LoadFinanceDataAsync(int organizationId)
    {
        return Work.GetCollection<FinanceData>()
            .Find(x => x.OrganizationId == organizationId)
            .ToListAsync();
    }

    private List<BaseAmountShort> GetVariables(Employee employee, PeriodCalendar calendar, HoursDetails hour,
        decimal avgSalary)
    {
        var periodDate = calendar.Period.ToDateTime();
        var currentSalary = employee.Salaries.FirstOrDefault(x =>
            x.DateFrom <= periodDate &&
            (!x.DateTo.HasValue || x.DateTo.Value > periodDate.AddMonths(1)))?.Amount ?? decimal.Zero;
        var paramsDict = new List<BaseAmountShort>()
        {
            new("S", currentSalary),
            new("AvgS", avgSalary),
            new("SumH", calendar.Hours.Summary),
            new("DH", calendar.Hours.Day),
            new("EH", calendar.Hours.Evening),
            new("NH", calendar.Hours.Night),
            new("HolH", calendar.Hours.HolidaySummary),
            new("HolDH", calendar.Hours.HolidayDay),
            new("HolEH", calendar.Hours.HolidayEvening),
            new("HolNH", calendar.Hours.HolidayNight),
            new("WD", calendar.WorkDays),
            new("SickL", calendar.SickLeave),
            new("VacL", calendar.VacationDays),
            new("Period", calendar.Period%100),
            new("TotH", hour.Summary),
            new("TotDH", hour.Day),
            new("TotEH", hour.Evening),
            new("TotNH", hour.Night),
            new("TotHolH", hour.HolidaySummary),
            new("TotHolDH", hour.HolidayDay),
            new("TotHolEH", hour.HolidayEvening),
            new("TotHolNH", hour.HolidayNight),
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

    private async Task<List<BaseAmountShort>> LoadBaseAmountsAsync(int period)
    {
        var filter = Builders<BaseAmount>.Filter;
        return (await Work.GetCollection<BaseAmount>()
            .Find(filter.Lte(x => x.DateFrom, period.ToDateTime()) & 
                  filter.Or(filter.Gte(x => x.DateTo, period.ToDateTime().AddMonths(1)),
                      filter.Eq(x => x.DateTo, null)))
            .Project(x => new BaseAmountShort(){ExprName = x.ExpressionName, Value = x.Value})
            .ToListAsync());
    }

    private Task<CalculationRegime> LoadRegimeAsync(int regimeId)
    {
        return _scheduleUnitOfWork.GetCollection<Regime>()
            .Find(x => x.Id == regimeId)
            .Project(x => new CalculationRegime()
            {
                RegimeId = x.Id,
                WorkDayDetails = x.WorkDayDetails,
                RestDays = x.RestDayDetails,
                IsCircle = x.IsCircle,
                StartDateInNextYear = x.StartDateInNextYear,
                StartDateInPreviousYear = x.StartDateInPreviousYear,
                StartDateInCurrentYear = x.StartDateInCurrentYear,
                ShiftsCount = x.ShiftsCount
            }).FirstOrDefaultAsync();
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

    private Task<List<Formula>> LoadFormulasAsync(int organizationId, IEnumerable<int> codes)
    {
        return Work.GetCollection<Formula>()
            .Find(x => x.OrganizationId == organizationId && codes.Contains(x.Code))
            .ToListAsync();
    }


    private class CalculationLoadedData
    {
        public ILookup<short, FinanceData> FinanceDict { get; set; }
        public ILookup<int, Formula> FormulaDict { get; set; }
        public Employee Employee { get; set; }
        public PeriodCalendar Calendar { get; set; }
        public CalculationRegime Regime { get; set; }
        public Dictionary<string, decimal> BaseAmounts { get; set; }
        public Dictionary<string, Formula[]> Formulas { get; set; }
    }

    private class BaseAmountShort
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