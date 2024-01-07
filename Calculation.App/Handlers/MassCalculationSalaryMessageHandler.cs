using AutoMapper;
using Calculation.App.Abstract;
using Calculation.App.Commands;
using Calculation.Data;
using Calculation.Data.Entities;
using Dictionary.Models;
using Microsoft.Extensions.Logging;
using MongoDB.Driver;
using Organization.Data.Data;
using Organization.Data.Entities;
using SalaryCalculation.Data.BaseModels;
using SalaryCalculation.Shared.Extensions.PeriodExtensions;
using Schedule.App.Helpers;
using Schedule.App.Models;
using Schedule.Data.BaseModels;
using Schedule.Data.Data;
using Schedule.Data.Entities;

namespace Calculation.App.Handlers;

public class MassCalculationSalaryMessageHandler: BaseSalaryCalculationMessageHandler<MassCalculationMessage>
{
    public MassCalculationSalaryMessageHandler(ICalculationUnitOfWork work,
        ILogger<BaseSalaryCalculationMessageHandler<MassCalculationMessage>> logger, IMapper mapper,
        IOrganizationUnitOfWork organizationUnitOfWork, IScheduleUnitOfWork scheduleUnitOfWork) : base(work, logger,
        mapper, organizationUnitOfWork, scheduleUnitOfWork)
    {
    }

    public override async Task HandleAsync(MassCalculationMessage msg)
    {
        await base.HandleAsync(msg);
        var calculationData = (CalculationLoadedData)Data;
        var task = Work.GetCollection<PaymentCard>()
            .DeleteManyAsync(x => calculationData.Employees.Select(x => x.Employee.Id).Contains(x.Employee.Id)
                                  && x.CalculationPeriod == msg.Period);

        var payments = new List<PaymentCard>();
        var lastId = await Work.NextValue<PaymentCard, int>(calculationData.Employees.Count);
        foreach (var employee in calculationData.Employees)
        {
            var paymentCard = await CalculateSalaryAsync(employee, msg, lastId--);
            payments.Add(paymentCard);
        }

        await task;
        
        await Work.GetCollection<PaymentCard>().InsertManyAsync(payments);
    }

    #region Main

    private async Task<PaymentCard> CalculateSalaryAsync(EmployeePersonalData personalData, MassCalculationMessage msg, int id)
    {
        personalData.Variables.ForEach(x => Data.BaseAmounts.Add(x.ExprName, x.Value));
        var result = new PaymentCard();
        result.Employee = new IdNamePair(personalData.Employee.Id, personalData.Employee.Name.ShortName);
        result.OrganizationId = msg.OrganizationId;
        result.CalculationDate = DateTime.Now;
        result.CalculationPeriod = msg.Period;
        
        var accrualOperationsTask = CreateOperationsAsync(Data.FinanceDict[1], personalData, 1);
        var maintenanceOperationsTask = CreateOperationsAsync(Data.FinanceDict[-1], personalData, -1);
        await Task.WhenAll(accrualOperationsTask, maintenanceOperationsTask);
        var accrualOperations = accrualOperationsTask.Result;
        var maintenanceOperations = maintenanceOperationsTask.Result;
        var operations = await InsertOperationsAsync(accrualOperations, maintenanceOperations, personalData.Employee.Id, msg.Period);
        result.AccrualAmount = accrualOperations.Sum(x => x.Amount);
        result.MaintenanceAmount = maintenanceOperations.Sum(x => x.Amount);
        result.PayedAmount = result.AccrualAmount - result.MaintenanceAmount;
        result.AccrualDetails = operations.Where(x => x.Sign == 1).Select(x => x.Id);
        result.MaintenanceDetails = operations.Where(x => x.Sign == -1).Select(x => x.Id);
        result.Id = id;
        personalData.Variables.ForEach(x => Data.BaseAmounts.Remove(x.ExprName));
        return result;
    }
    
    private async Task<List<Operation>> CreateOperationsAsync(IEnumerable<FinanceData> data,
        EmployeePersonalData personalData, int sign)
    {
        var ops = new List<Operation>();
        await Parallel.ForEachAsync(data, async (fin, token) =>
        {
            var allowedFormulas = Data.FormulaDict[fin.Code];
            var formula = EvaluateConditions(allowedFormulas);
            if (formula == null)
                return;
            var amount = EvaluateExpression(formula);
            var operation = new Operation()
            {
                Code = fin.Code,
                Name = fin.Name,
                EmployeeId = personalData.Employee.Id,
                Amount = amount,
                Sign = sign,
                Period = personalData.Calendar.Period,
                OrganizationId = personalData.Employee.Organization.Id
            };
            ops.Add(operation);
        });

        return ops;
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

    #endregion
    

    #region Data loading

    protected override async Task<BaseLoadedData> LoadCacheDataAsync(MassCalculationMessage msg)
    {
        var data = new CalculationLoadedData();
        
        var financeData = await LoadFinanceDataAsync(msg.OrganizationId);
        var employeeTask = LoadEmployeeDataAsync(msg);
        var formulaTask = LoadFormulasAsync(msg.OrganizationId, financeData.Select(x => x.Code));
        var baseAmountTask = LoadBaseAmountsAsync(msg.Period);
        await Task.WhenAll(formulaTask, employeeTask, baseAmountTask);
        var employees = employeeTask.Result;
        var periodCalendarsTask = LoadPeriodCalendarsAsync(employees.Select(x => x.Id), msg.Period);
        var regimesTask = LoadRegimesAsync(employees.Select(x => x.RegimeId).Distinct());
        await Task.WhenAll(periodCalendarsTask, regimesTask);
        var formulas = formulaTask.Result;
        
        data.BaseAmounts = baseAmountTask.Result.Concat(GetBaseVariables()).ToDictionary(k => k.ExprName, v => v.Value);
        data.FinanceDict = financeData.ToLookup(k => k.Sign);
        data.FormulaDict = formulas.ToLookup(k => k.Code);
        data.Formulas = formulas.GroupBy(k => k.ExpressionName).ToDictionary(k => k.Key, v => v.ToArray());

        var periodCalendarsDict = periodCalendarsTask.Result.ToDictionary(k => k.EmployeeId);
        var regimes = regimesTask.Result;
        var hourDetailsByRegime = new Dictionary<int,Dictionary<int, HoursDetails>>(); // key1 - regimeID, key2 - shiftNumber
        foreach (var regime in regimes)
            hourDetailsByRegime.Add(regime.RegimeId, await RegimeHelper.GetHoursDetailAsync(regime, msg.Period));
        var avgSalaries = await LoadAverageDaySalaryAsync(employees.Select(x => x.Id), msg.Period);
        
        data.Regimes = regimes.ToDictionary(k => k.RegimeId);
        data.Employees = employees.Select(x =>
        {
            if (periodCalendarsDict.TryGetValue(x.Id, out var calendar))
            {
                avgSalaries.TryGetValue(x.Id, out var avg);
                return new EmployeePersonalData()
                {
                    Employee = x,
                    Calendar = calendar,
                    Variables = GetCustomVariables(x, calendar, hourDetailsByRegime[x.RegimeId][x.ShiftNumber], avg)
                };
                
            }
            return null;
        }).Where(x => x != null).ToList();

        return data;
    }
    
    private async Task<Dictionary<int, decimal>> LoadAverageDaySalaryAsync(IEnumerable<int> ids, int period)
    {
        var periodFrom = period / 100 * 100 + 1;
        var periodTo = period.PreviousPeriod();
        var calendarsTask = ScheduleUnitOfWork.GetCollection<PeriodCalendar>()
            .Find(x => ids.Contains(x.EmployeeId) && x.Period >= periodFrom && x.Period <= periodTo)
            .Project(x => new {x.EmployeeId, x.WorkDays})
            .ToListAsync();
        var paymentsTask = Work.GetCollection<PaymentCard>()
            .Find(x => ids.Contains(x.Employee.Id) && x.CalculationPeriod >= periodFrom && x.CalculationPeriod <= periodTo)
            .Project(x => new {x.Employee.Id, x.AccrualAmount})
            .ToListAsync();
        await Task.WhenAll(calendarsTask, paymentsTask);
        var calendar = calendarsTask.Result.GroupBy(k => k.EmployeeId)
            .ToDictionary(k => k.Key, v => v.Sum(x => x.WorkDays));
        var payment = paymentsTask.Result.GroupBy(k => k.Id)
            .ToDictionary(k => k.Key, v => v.Sum(x => x.AccrualAmount));;
        var dict = new Dictionary<int, decimal>();
        foreach (var id in ids)
        {
            calendar.TryGetValue(id, out var days);
            payment.TryGetValue(id, out var pay);
            dict.Add(id, days != 0 ? pay / days : decimal.Zero);
        }

        return dict;
    }

    private List<BaseAmountShort> GetCustomVariables(Employee employee, PeriodCalendar calendar, HoursDetails hour, decimal avgSalary)
    {
        var periodDate = calendar.Period.ToDateTime();
        var currentSalary = employee.Salaries.FirstOrDefault(x =>
            x.DateFrom <= periodDate &&
            (!x.DateTo.HasValue || x.DateTo.Value > periodDate.AddMonths(1)))?.Amount ?? decimal.Zero;
        var paramsDict = new List<BaseAmountShort>()
        {
            new("S", currentSalary),
            new("AvgS", Math.Round(avgSalary, 3)),
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
            new("Period", calendar.Period % 100),
            new("TotH", hour.Summary),
            new("TotDH", hour.Day),
            new("TotEH", hour.Evening),
            new("TotNH", hour.Night),
            new("TotHolH", hour.HolidaySummary),
            new("TotHolDH", hour.HolidayDay),
            new("TotHolEH", hour.HolidayEvening),
            new("TotHolNH", hour.HolidayNight),
        };
        return paramsDict;
    }

    private Task<List<PeriodCalendar>> LoadPeriodCalendarsAsync(IEnumerable<int> employeeIds, int period)
    {
        var filter = Builders<PeriodCalendar>.Filter;
        return ScheduleUnitOfWork.GetCollection<PeriodCalendar>()
            .Find(filter.In(x => x.EmployeeId, employeeIds) & filter.Eq(x => x.Period, period))
            .ToListAsync();
    }
    
    private Task<List<CalculationRegime>> LoadRegimesAsync(IEnumerable<int> ids)
    {
        var filter = Builders<Regime>.Filter;
        return ScheduleUnitOfWork.GetCollection<Regime>()
            .Find(filter.In(x => x.Id, ids))
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
            })
            .ToListAsync();
    }

    private Task<List<FinanceData>> LoadFinanceDataAsync(int organizationId)
    {
        return Work.GetCollection<FinanceData>()
            .Find(x => x.OrganizationId == organizationId)
            .ToListAsync();
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
    
    private Task<List<Formula>> LoadFormulasAsync(int organizationId, IEnumerable<int> codes)
    {
        return Work.GetCollection<Formula>()
            .Find(x => x.OrganizationId == organizationId && codes.Contains(x.Code))
            .ToListAsync();
    }
    
    private Task<List<Employee>> LoadEmployeeDataAsync(MassCalculationMessage msg)
    {
        var periodDate = msg.Period.ToDateTime();
        var builder = Builders<Employee>.Filter;
        var filter = builder.Eq(x => x.Organization.Id, msg.OrganizationId) &
                     builder.Lt(x => x.DateFrom, periodDate.AddMonths(1)) &
                     builder.Or(builder.Eq(x => x.DateTo, null),
                         builder.Gt(x => x.DateTo, periodDate.AddMonths(1)));
        if (msg.OrganizationUnitId.HasValue)
            filter &= builder.Eq(x => x.OrganizationUnit.Id, msg.OrganizationUnitId.Value);
        if (msg.PositionId.HasValue)
            filter &= builder.Eq(x => x.Position.Id, msg.PositionId.Value);
        if (msg.RegimeId.HasValue)
            filter &= builder.Eq(x => x.RegimeId, msg.RegimeId.Value);

        return OrganizationUnitOfWork.GetCollection<Employee>()
            .Find(filter)
            .ToListAsync();
    }

    #endregion

    #region nested Classes

    protected class CalculationLoadedData : BaseLoadedData
    {
        public List<EmployeePersonalData> Employees { get; set; }
        public Dictionary<int, CalculationRegime> Regimes { get; set; }
    }
    
    protected class EmployeePersonalData
    {
        public Employee Employee { get; set; }
        public List<BaseAmountShort> Variables { get; set; }
        public PeriodCalendar Calendar { get; set; }
    }

    #endregion
}