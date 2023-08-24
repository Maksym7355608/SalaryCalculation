using AutoMapper;
using Microsoft.Extensions.Logging;
using MongoDB.Driver;
using MongoDB.Driver.Linq;
using Progress.App;
using SalaryCalculation.Shared.Extensions.PeriodExtensions;
using Schedule.App.Abstract;
using Schedule.App.Commands;
using Schedule.Data.BaseModels;
using Schedule.Data.Data;
using Schedule.Data.Entities;
using Schedule.Data.Enums;

namespace Schedule.App.Handlers;

public class ScheduleReaderLogic : BaseScheduleCommandHandler, IScheduleReaderLogic
{
    public ScheduleReaderLogic(IScheduleUnitOfWork work, ILogger<ScheduleReaderLogic> logger, IMapper mapper) : base(work, logger, mapper)
    {
    }

    public async Task<bool> CalculatePeriodCalendarAsync(int employeeId, int period, int regimeId)
    {
        var dateFrom = period.ToDateTime();
        var dateTo = dateFrom.AddMonths(1).AddDays(-1);
        var filter = GetPeriodCalendarFilter(dateFrom, dateTo, employeeId);

        var empDays = await Work.GetCollection<EmpDay>()
            .Find(filter)
            .ToListAsync();

        var calendar = CreateCalendar(empDays, period, regimeId);

        var existCalendar = await Work.GetCollection<PeriodCalendar>()
            .UpdateOneAsync(x => x.EmployeeId == employeeId && x.Period == period,
                Builders<PeriodCalendar>.Update.Set(x => x, calendar));
        if (existCalendar.ModifiedCount == 0)
        {
            await Work.GetCollection<PeriodCalendar>().InsertOneAsync(calendar);
            return true;
        }

        return existCalendar.ModifiedCount > 0;
    }

    public async Task<string> MassCalculatePeriodCalendarAsync(PeriodCalendarMassCalculateCommand command)
    {
        var progressId = Guid.NewGuid().ToString();
        var progressMsg = new ProgressCreateMessage()
        {
            ProgressId = progressId,
        };
        await Work.MessageBroker.PublishAsync(progressMsg);

        var msg = new PeriodCalendarMassCalculateMessage()
        {
            PeriodFrom = command.PeriodFrom,
            PeriodTo = command.PeriodTo,
            OrganizationId = command.OrganizationId,
            RegimeIds = command.RegimeIds
        };
        await Work.MessageBroker.PublishAsync(msg);
        return progressId;
    }

    private PeriodCalendar CreateCalendar(List<EmpDay> empDays, int period, int regimeId)
    {
        return new PeriodCalendar()
        {
            EmployeeId = empDays.First().EmployeeId,
            Hours = new HoursDetail()
            {
                Summary = empDays.Sum(x => x.Hours.Summary),
                Day = empDays.Sum(x => x.Hours.Day),
                Evening = empDays.Sum(x => x.Hours.Evening),
                Night = empDays.Sum(x => x.Hours.Night),
                Holiday = empDays.Sum(x => x.Hours.Holiday)
            },
            OrganizationId = empDays.First().OrganizationId,
            Period = period,
            SickLeave = empDays.Where(x => x.DayType == (int)EDayType.Sick).Count(),
            VacationDays = empDays.Where(x => x.DayType == (int)EDayType.Holiday).Count(),
            WorkDays = empDays.Where(x => x.DayType == (int)EDayType.Work).Count(),
            RegimeId = regimeId,
        };
        
    }

    private FilterDefinition<EmpDay> GetPeriodCalendarFilter(DateTime dateFrom, DateTime dateTo, int employeeId)
    {
        var filter = Builders<EmpDay>.Filter;
        return filter.Eq(x => x.EmployeeId, employeeId) &
               filter.Gte(x => x.Date, dateFrom) &
               filter.Lte(x => x.Date, dateTo);
    }


    public async Task<bool> QuickSettingWorkDays(WorkDaysSettingFilter filter)
    {
        var workDays = CreateDays(filter);
        //TODO: Написати універсальний механізм, який буде записувати нові, та оновлювати старі записи робочого дня
        return false;
    }

    public async Task<bool> QuickSettingRestDays(RestDaysSettingFilter filter)
    {
        var restDays = CreateDays(filter);
        return false;
    }

    private List<EmpDay> CreateDays<T>(T filter) where T: RestDaysSettingFilter
    {
        var workedDates = new List<DateTime>(); //TODO: Тут потрібно зробити функцію, яка шукає всі робочі дні за задані дати
    
        var restDates = new List<DateTime>(); //TODO: Тут потрібно зробити функцію, яка шукає всі неробочі дні за задані дати

        var autoEmpDays = new List<EmpDay>();
        //TODO: Тут потрібно зробити логіку створення сутностей EmpDays відносно робочих/неробочих дат, які ми отримали вище, а також відносно режиму роботи.
        return autoEmpDays;
    }
}