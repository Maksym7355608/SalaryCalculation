using System.Data;
using AutoMapper;
using Microsoft.Extensions.Logging;
using MongoDB.Bson;
using MongoDB.Driver;
using Organization.Data.Data;
using Organization.Data.Entities;
using SalaryCalculation.Data.BaseModels;
using SalaryCalculation.Shared.Common.Validation;
using SalaryCalculation.Shared.Extensions.EnumExtensions;
using SalaryCalculation.Shared.Extensions.MoreLinq;
using SalaryCalculation.Shared.Extensions.PeriodExtensions;
using Schedule.App.Abstract;
using Schedule.App.Commands;
using Schedule.App.Dto;
using Schedule.Data.BaseModels;
using Schedule.Data.Data;
using Schedule.Data.Entities;
using Schedule.Data.Enums;

namespace Schedule.App.Handlers;

public class ScheduleCommandHandler : BaseScheduleCommandHandler, IScheduleCommandHandler
{
    private IOrganizationUnitOfWork _organizationUnitOfWork;
    private IMongoCollection<Regime> _regimeCollection;
    private IMongoCollection<EmpDay> _dayCollection;
    
    public ScheduleCommandHandler(IScheduleUnitOfWork work, ILogger<ScheduleCommandHandler> logger, IMapper mapper,
        IOrganizationUnitOfWork organizationUnitOfWork) : base(work, logger, mapper)
    {
        _regimeCollection = Work.GetCollection<Regime>();
        _dayCollection = Work.GetCollection<EmpDay>();
        _organizationUnitOfWork = organizationUnitOfWork;
    }

    public async Task<IEnumerable<RegimeDto>> GetRegimesAsync(int organizationId)
    {
        var regimes = await _regimeCollection
            .Find(x => x.OrganizationId == organizationId)
            .ToListAsync();
        return Mapper.Map<IEnumerable<RegimeDto>>(regimes);
    }

    public async Task<RegimeDto> GetRegimeAsync(int id)
    {
        var regime = await _regimeCollection
            .Find(x => x.Id == id)
            .FirstOrDefaultAsync();
        if (regime == null)
            throw new EntityNotFoundException("Regime with id {0} not found", id.ToString());
        return Mapper.Map<RegimeDto>(regime);
    }

    public async Task<bool> CreateRegimeAsync(RegimeCreateCommand command)
    {
        var newRegime = Mapper.Map<Regime>(command);
        if (Work.GetCollection<Regime>().Find(x => x.Code == newRegime.Code).Any())
            throw new DuplicateNameException("Regime with the same code exist");
        newRegime.Id = (int)_regimeCollection.NewNumberId();
        await _regimeCollection
            .InsertOneAsync(newRegime);
        return true;
    }

    public async Task<bool> UpdateRegimeAsync(RegimeUpdateCommand command)
    {
        if (!Work.GetCollection<Regime>().Find(x => x.Id == command.Id).Any())
            throw new EntityNotFoundException("Regime with id {0} not found", command.Id.ToString());

        var update = Builders<Regime>.Update
            .Set(x => x.Name, command.Name)
            .Set(x => x.WorkDayDetails, Mapper.Map<IEnumerable<WorkDayDetail>>(command.WorkDayDetails))
            .Set(x => x.RestDayDetails, command.RestDayDetails)
            .Set(x => x.StartDateInCurrentYear, command.StartDateInCurrentYear)
            .Set(x => x.StartDateInNextYear, command.StartDateInNextYear);
        var res = await Work.GetCollection<Regime>()
            .UpdateOneAsync(x => x.Id == command.Id, update);
        return res.ModifiedCount > 0;
    }

    public async Task<bool> DeleteRegimeAsync(int id)
    {
        var result = await Work.GetCollection<Regime>()
            .DeleteOneAsync(x => x.Id == id);
        return result.DeletedCount > 0;
    }

    public async Task<IEnumerable<WorkDayDetailDto>> GetWorkDaysRegimeAsync(int regimeId)
    {
        var workDaysRegime = await Work.GetCollection<Regime>()
            .Find(x => x.Id == regimeId)
            .Project(x => x.WorkDayDetails)
            .FirstOrDefaultAsync();
        if (workDaysRegime == null)
            throw new EntityNotFoundException("Regime with id {0} not found", regimeId.ToString());
        return Mapper.Map<IEnumerable<WorkDayDetailDto>>(workDaysRegime);
    }

    public async Task<bool> UpdateWorkDayRegimeAsync( WorkDayRegimeUpdateCommand command)
    {
        if (!Work.GetCollection<Regime>().Find(x => x.Id == command.RegimeId).Any())
            throw new EntityNotFoundException("Regime with id {0} not found", command.RegimeId.ToString());
        
        var workDays = Mapper.Map<IEnumerable<WorkDayDetail>>(command.WorkDayDetails);
        var restDays = command.RestDayDetails;

        var res = await Work.GetCollection<Regime>()
            .UpdateOneAsync(x => x.Id == command.RegimeId, Builders<Regime>.Update
                .Set(x => x.WorkDayDetails, workDays)
                .Set(x => x.RestDayDetails, restDays));
        return res.ModifiedCount > 0;
    }

    public async Task<IEnumerable<PeriodCalendarDto>> GetPeriodsCalendarByEmployeeAsync(int employeeId)
    {
        var calendar = await Work.GetCollection<PeriodCalendar>()
            .Find(x => x.EmployeeId == employeeId)
            .ToListAsync();
        return Mapper.Map<IEnumerable<PeriodCalendarDto>>(calendar);
    }

    public async Task<IEnumerable<PeriodCalendarDto>> SearchPeriodsCalendarAsync(PeriodCalendarSearchCommand command)
    {
        var filter = await GetPeriodCalendarSearchFilterAsync(command);
        var calendar = await Work.GetCollection<PeriodCalendar>()
            .Find(filter)
            .ToListAsync();
        return Mapper.Map<IEnumerable<PeriodCalendarDto>>(calendar);
    }

    private async Task<FilterDefinition<PeriodCalendar>> GetPeriodCalendarSearchFilterAsync(PeriodCalendarSearchCommand command)
    {
        var filterBuilder = Builders<PeriodCalendar>.Filter;
        var definition = new List<FilterDefinition<PeriodCalendar>>()
        {
            filterBuilder.Eq(x => x.OrganizationId, command.OrganizationId),
            filterBuilder.Gte(x => x.Period, command.PeriodFrom),
        };
        if (!string.IsNullOrWhiteSpace(command.EmployeeNumber))
        {
            var employeeId = await _organizationUnitOfWork.GetCollection<Employee>()
                .Find(Builders<Employee>.Filter.Regex(x => x.RollNumber, new BsonRegularExpression(command.EmployeeNumber, "i")))
                .Project(x => x.Id)
                .FirstOrDefaultAsync();
            if (employeeId == null)
                throw new EntityNotFoundException("Employee with roll number {0} was not found", command.EmployeeNumber);
            definition.Add(filterBuilder.Eq(x => x.EmployeeId, employeeId));
        }
        
        if(command.PeriodTo.HasValue)
            definition.Add(filterBuilder.Lte(x => x.Period, command.PeriodTo.Value));
        if(command.RegimeId.HasValue)
            definition.Add(filterBuilder.Eq(x => x.RegimeId, command.RegimeId.Value));
        
        return filterBuilder.And(definition);
    }

    public async Task<PeriodCalendarDto> GetPeriodCalendarAsync(int employeeId, int period)
    {
        var calendar = await Work.GetCollection<PeriodCalendar>()
            .Find(x => x.EmployeeId == employeeId && x.Period == period)
            .FirstOrDefaultAsync();
        if (calendar == null)
            throw new EntityNotFoundException("Employee {0} don`t have work calendar from period {1}",
                employeeId.ToString(), period.ToShortPeriodString());
        var dto = Mapper.Map<PeriodCalendarDto>(calendar);
        dto.Regime = await Work.GetCollection<Regime>()
            .Find(x => x.Id == calendar.RegimeId)
            .Project(x => new IdNamePair()
            {
                Id = x.Id, Name = x.Name
            }).FirstAsync();
        return dto;
    }

    public async Task<IEnumerable<EmployeeScheduleDto>> GetWorkDaysAsync(WorkDaySearchCommand command)
    {
        var filter = await GetEmpDayFilterAsync(command);
        var empDays = await Work.GetCollection<EmpDay>()
            .Find(filter)
            .ToListAsync();
        var employeeNames = await _organizationUnitOfWork.GetCollection<Employee>()
            .Find(Builders<Employee>.Filter.In(x => x.Id, empDays.Select(x => x.EmployeeId)))
            .Project(x => new
            {
                x.Id,
                Name = x.Name
            })
            .ToListAsync();
        var employeeNamesDict = employeeNames.ToDictionary(k => k.Id, 
            v => $"{v.Name.LastName} {v.Name.FirstName} {v.Name.MiddleName}");
        var dto = empDays.GroupBy(k => k.EmployeeId)
            .Select(model => new EmployeeScheduleDto()
            {
                Id = model.Key,
                Name = employeeNamesDict[model.Key],
                Period = command.Period,
                Schedule = model.Select(s => new EmpDayShortDto()
                {
                    Day = s.Date.ToShortUADateString(),
                    Work = s.DayType == (int)EDayType.Work
                        ? s.Hours.Summary.ToString()
                        : ((EDayType)s.DayType).GetDescription()
                })
            });
        return dto;
    }
    
    private async Task<FilterDefinition<EmpDay>> GetEmpDayFilterAsync(WorkDaySearchCommand command)
    {
        var filter = Builders<EmpDay>.Filter;
        var definition = new List<FilterDefinition<EmpDay>>()
        {
            filter.Eq(x => x.OrganizationId, command.OrganizationId),
            filter.Gte(x => x.Date, command.Period.ToDateTime()),
            filter.Lt(x => x.Date, command.Period.ToDateTime().AddMonths(1)),
        };
        
        var empFilter = Builders<Employee>.Filter;
        var empDefinition = new List<FilterDefinition<Employee>>()
        {
            empFilter.Eq(x => x.Organization.Id, command.OrganizationId)
        };
        if (command.OrganizationUnitIds?.Any() ?? false)
            empDefinition.Add(empFilter.In(x => x.OrganizationUnit.Id, command.OrganizationUnitIds));
        if (command.PositionIds?.Any() ?? false)
            empDefinition.Add(empFilter.In(x => x.Position.Id, command.PositionIds));
        var employeeIds = await _organizationUnitOfWork.GetCollection<Employee>()
            .Find(empFilter.And(empDefinition))
            .Project(x => x.Id)
            .ToListAsync();
        
        if(employeeIds?.Count > 0)
            definition.Add(filter.In(x => x.EmployeeId, employeeIds));

        return filter.And(definition);
    }

    public async Task<IEnumerable<EmpDayDto>> GetEmpDaysByEmployeeAsync(int employeeId, int period)
    {
        var dateFrom = period.ToDateTime();
        var dateTo = dateFrom.AddMonths(1);
        var empDays = await _dayCollection
            .Find(x => x.EmployeeId == employeeId && x.Date >= dateFrom && x.Date < dateTo)
            .ToListAsync();
        var dto = empDays.Select(x => new EmpDayDto
        {
            EmployeeId = employeeId,
            Date = x.Date.ToShortUADateString(),
            Day = x.Hours.Day,
            Evening = x.Hours.Evening,
            Night = x.Hours.Night,
            Summary = x.DayType == (int)EDayType.Work
                ? x.Hours.Summary.ToString()
                : ((EDayType)x.DayType).GetDescription(),
        });
        return dto;
    }

    public async Task<bool> SetWorkDayAsync(WorkDayCreateCommand command)
    {
        var empDay = Mapper.Map<EmpDay>(command);
        var isExist = await _dayCollection
            .ReplaceOneAsync(x => x.EmployeeId == command.EmployeeId && x.OrganizationId == command.OrganizationId &&
                                 x.Date == command.Date, empDay);
        if (isExist.ModifiedCount == 0)
        {
            empDay.Id = _dayCollection.NewNumberId();
            await _dayCollection.InsertOneAsync(empDay);
            return true;
        }

        return isExist.ModifiedCount > 0;
    }
    
    public async Task<bool> CalculatePeriodCalendarAsync(int employeeId, int period, int regimeId)
    {
        var dateFrom = period.ToDateTime();
        var dateTo = dateFrom.AddMonths(1);
        var filter = GetPeriodCalendarFilter(dateFrom, dateTo, new []{employeeId});

        var empDays = await Work.GetCollection<EmpDay>()
            .Find(filter)
            .ToListAsync();

        var calendar = CreateCalendar(empDays, period, regimeId);

        var existCalendar = await Work.GetCollection<PeriodCalendar>()
            .ReplaceOneAsync(x => x.EmployeeId == employeeId && x.Period == period, calendar);
        if (existCalendar.ModifiedCount == 0)
        {
            await Work.GetCollection<PeriodCalendar>().InsertOneAsync(calendar);
            return true;
        }

        return existCalendar.ModifiedCount > 0;
    }

    public async Task MassCalculatePeriodCalendarAsync(PeriodCalendarMassCalculateCommand command)
    {
        var dateFrom = command.PeriodFrom.ToDateTime();
        var dateTo = command.PeriodTo.HasValue ? command.PeriodTo.Value.ToDateTime().AddMonths(1) : dateFrom.AddMonths(1);
        var builder = Builders<PeriodCalendar>.Filter;
        if (command.EmployeeIds == null)
            command.EmployeeIds = (await _organizationUnitOfWork.GetCollection<Employee>()
                .Find(x => x.Organization.Id == command.OrganizationId)
                .Project(x => x.Id)
                .ToListAsync()).ToArray();
        
        var filter = GetPeriodCalendarFilter(dateFrom, dateTo, command.EmployeeIds);
        
        var pcFilter = Builders<PeriodCalendar>.Filter;
        var def = new List<FilterDefinition<PeriodCalendar>>()
        {
            pcFilter.Gte(x => x.Period, command.PeriodFrom),
            pcFilter.Lt(x => x.Period, command.PeriodTo ?? command.PeriodFrom.NextPeriod())
        };
        if(command.EmployeeIds != null && command.EmployeeIds.Length > 0)
            def.Add(pcFilter.In(x => x.EmployeeId, command.EmployeeIds));

        var existCalendarsTask = Work.GetCollection<PeriodCalendar>()
            .Find(pcFilter.And(def))
            .Project(x => new
            {
                EmployeeId = x.EmployeeId,
                Period = x.Period
            })
            .ToListAsync();
        var empDaysTask = Work.GetCollection<EmpDay>()
            .Find(filter)
            .ToListAsync();
        var regimesTask = GetRegimesDictionaryAsync(command.EmployeeIds);
        await Task.WhenAll(existCalendarsTask, empDaysTask, regimesTask);

        var empDaysDict = empDaysTask.Result.ToLookup(k => k.EmployeeId, v => v);
        var regimesDict = regimesTask.Result;
        var existCalendarsDict = existCalendarsTask.Result.ToLookup(k => k.EmployeeId, v => v.Period);

        var writeResult = new List<WriteModel<PeriodCalendar>>();
        for (var current = command.PeriodFrom; current <= (command.PeriodTo ?? command.PeriodFrom); current = current.NextPeriod())
        {
            foreach (var employeeId in command.EmployeeIds)
            {
                var calendar = CreateCalendar(empDaysDict[employeeId], current, regimesDict[employeeId]);
                var search = builder.Eq(x => x.EmployeeId, employeeId)
                             & builder.Eq(x => x.Period, current);
                var isExist = existCalendarsDict[employeeId].Contains(current);
                if(isExist)
                    writeResult.Add(new ReplaceOneModel<PeriodCalendar>(search, calendar));
                else
                    writeResult.Add(new InsertOneModel<PeriodCalendar>(calendar));
            }
        }

        if (writeResult.Count > 0)
            await Work.GetCollection<PeriodCalendar>().BulkWriteAsync(writeResult);
    }

    public async Task QuickSettingDaysAsync(DaysSettingMessage msg)
    {
        await Work.MessageBroker.PublishAsync(msg);
    }

    private async Task<Dictionary<int, int>> GetRegimesDictionaryAsync(int[] employeeIds)
    {
        var employees = await _organizationUnitOfWork.GetCollection<Employee>()
            .Find(Builders<Employee>.Filter.In(x => x.Id, employeeIds))
            .Project(x => new
            {
                x.Id,
                x.RegimeId
            })
            .ToListAsync();
        return employees.ToDictionary(k => k.Id, v => v.RegimeId);
    }

    public PeriodCalendar CreateCalendar(IEnumerable<EmpDay> empDays, int period, int regimeId)
    {
        empDays = empDays.Where(x => x.Date >= period.ToDateTime() && x.Date < period.ToDateTime().AddMonths(1))
            .ToArray();
        var holidayHours = empDays.Where(x => x.Hours.Holiday).Select(x => x.Hours).ToArray();
        return new PeriodCalendar()
        {
            EmployeeId = empDays.First().EmployeeId,
            Hours = new HoursDetails()
            {
                Summary = empDays.Sum(x => x.Hours.Summary),
                Day = empDays.Sum(x => x.Hours.Day),
                Evening = empDays.Sum(x => x.Hours.Evening),
                Night = empDays.Sum(x => x.Hours.Night),
                HolidaySummary = holidayHours.Sum(x => x.Summary),
                HolidayDay = holidayHours.Sum(x => x.Day),
                HolidayNight = holidayHours.Sum(x => x.Night),
                HolidayEvening = holidayHours.Sum(x => x.Evening),
            },
            OrganizationId = empDays.First().OrganizationId,
            Period = period,
            SickLeave = empDays.Where(x => x.DayType == (int)EDayType.Sick).Count(),
            VacationDays = empDays.Where(x => x.DayType == (int)EDayType.Holiday).Count(),
            WorkDays = empDays.Where(x => x.DayType == (int)EDayType.Work).Count(),
            RegimeId = regimeId,
        };
    }

    private FilterDefinition<EmpDay> GetPeriodCalendarFilter(DateTime dateFrom, DateTime dateTo, int[]? employeeIds)
    {
        var filter = Builders<EmpDay>.Filter;
        var def = new List<FilterDefinition<EmpDay>>()
        {
            filter.Gte(x => x.Date, dateFrom),
            filter.Lt(x => x.Date, dateTo)
        };
        if(employeeIds != null && employeeIds.Length > 0)
            def.Add(filter.In(x => x.EmployeeId, employeeIds));
        return filter.And(def);
    }
}