using System.Data;
using AutoMapper;
using Microsoft.Extensions.Logging;
using MongoDB.Driver;
using Organization.Data.Data;
using Organization.Data.Entities;
using SalaryCalculation.Shared.Common.Validation;
using SalaryCalculation.Shared.Extensions.PeriodExtensions;
using Schedule.App.Abstract;
using Schedule.App.Commands;
using Schedule.App.Dto;
using Schedule.Data.BaseModels;
using Schedule.Data.Data;
using Schedule.Data.Entities;

namespace Schedule.App.Handlers;

public class ScheduleCommandHandler : BaseScheduleCommandHandler, IScheduleCommandHandler
{
    private IOrganizationUnitOfWork _organizationUnitOfWork;
    
    public ScheduleCommandHandler(IScheduleUnitOfWork work, ILogger<ScheduleCommandHandler> logger, IMapper mapper,
        IOrganizationUnitOfWork organizationUnitOfWork) : base(work, logger, mapper)
    {
        _organizationUnitOfWork = organizationUnitOfWork;
    }

    public async Task<IEnumerable<RegimeDto>> GetRegimesAsync(int organizationId)
    {
        var regimes = await Work.GetCollection<Regime>()
            .Find(x => x.OrganizationId == organizationId)
            .ToListAsync();
        return Mapper.Map<IEnumerable<RegimeDto>>(regimes);
    }

    public async Task<RegimeDto> GetRegimeAsync(int id)
    {
        var regime = await Work.GetCollection<Regime>()
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
        await Work.GetCollection<Regime>()
            .InsertOneAsync(newRegime);
        return true;
    }

    public async Task<bool> UpdateRegimeAsync(RegimeUpdateCommand command)
    {
        var regime = Mapper.Map<Regime>(command);
        if (!Work.GetCollection<Regime>().Find(x => x.Id == regime.Id).Any())
            throw new EntityNotFoundException("Regime with id {0} not found", command.Id.ToString());

        var res = await Work.GetCollection<Regime>()
            .UpdateOneAsync(x => x.Id == regime.Id, Builders<Regime>.Update.Set(x => x, regime));
        return res.ModifiedCount > 0;
    }

    public async Task<bool> DeleteRegimeAsync(int id)
    {
        var result = await Work.GetCollection<Regime>()
            .DeleteOneAsync(x => x.Id == id);
        return result.DeletedCount > 0;
    }

    public async Task<WorkDayDetailDto> GetWorkDaysRegimeAsync(int regimeId)
    {
        var workDaysRegime = await Work.GetCollection<Regime>()
            .Find(x => x.Id == regimeId)
            .Project(x => x.WorkDayDetails)
            .FirstOrDefaultAsync();
        if (workDaysRegime == null)
            throw new EntityNotFoundException("Regime with id {0} not found", regimeId.ToString());
        return Mapper.Map<WorkDayDetailDto>(workDaysRegime);
    }

    public async Task<bool> UpdateWorkDayRegimeAsync(int regimeId, WorkDayRegimeUpdateCommand command)
    {
        var workDaysRegime = Mapper.Map<WorkDayDetail>(command);
        if (!Work.GetCollection<Regime>().Find(x => x.Id == regimeId).Any())
            throw new EntityNotFoundException("Regime with id {0} not found", regimeId.ToString());

        var res = await Work.GetCollection<Regime>()
            .UpdateOneAsync(x => x.Id == regimeId, Builders<Regime>.Update.Set($"{nameof(Regime)}.WorkDayDetails.", workDaysRegime));//TODO
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
        if (command.EmployeeNumber.HasValue)
        {
            var employeeId = await _organizationUnitOfWork.GetCollection<Employee>()
                .Find(x => x.RollNumber == command.EmployeeNumber.Value)
                .Project(x => x.Id)
                .FirstOrDefaultAsync();
            if (employeeId == 0)
                throw new EntityNotFoundException("Employee with roll number {0} was not found",
                    command.EmployeeNumber.Value.ToString());
            definition.Add(filterBuilder.Eq(x => x.EmployeeId, employeeId));
        }
        
        if(command.PeriodTo.HasValue)
            definition.Add(filterBuilder.Lte(x => x.Period, command.PeriodTo.Value));
        if(command.RegimeId.HasValue)
            definition.Add(filterBuilder.Eq(x => x.RegimeId, command.RegimeId.Value));
        if (command.VacationDays.HasValue)
            definition.Add(filterBuilder.Eq(x => x.VacationDays, command.VacationDays.Value));
        if (command.SickLeave.HasValue)
            definition.Add(filterBuilder.Eq(x => x.SickLeave, command.SickLeave.Value));
        
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
        return Mapper.Map<PeriodCalendarDto>(calendar);
    }

    public async Task<IEnumerable<EmpDayDto>> GetWorkDaysAsync(WorkDaySearchCommand command)
    {
        var filter = GetEmpDayFilter(command);
        var empDays = await Work.GetCollection<EmpDay>()
            .Find(filter)
            .ToListAsync();
        return Mapper.Map<IEnumerable<EmpDayDto>>(empDays);
    }

    private FilterDefinition<EmpDay> GetEmpDayFilter(WorkDaySearchCommand command)
    {
        var filter = Builders<EmpDay>.Filter;
        var definition = new List<FilterDefinition<EmpDay>>()
        {
            filter.Eq(x => x.OrganizationId, command.OrganizationId),
            filter.Gte(x => x.Date, command.DateFrom),
        };
        if(command.DateTo.HasValue)
            definition.Add(filter.Lte(x => x.Date, command.DateTo.Value));
        if(command.DayType.HasValue)
            definition.Add(filter.Eq(x => x.DayType, (int)command.DayType.Value));
        if(command.EmployeeIds != null && command.EmployeeIds.Any())
            definition.Add(filter.In(x => x.EmployeeId, command.EmployeeIds));

        return filter.And(definition);
    }

    public async Task<bool> SetWorkDayAsync(WorkDayCreateCommand command)
    {
        var empDay = Mapper.Map<EmpDay>(command);
        var isExist = await Work.GetCollection<EmpDay>()
            .UpdateOneAsync(x => x.EmployeeId == command.EmployeeId && x.OrganizationId == command.OrganizationId &&
                                 x.Date == command.Date, Builders<EmpDay>.Update.Set(x => x, empDay));
        if (isExist.ModifiedCount == 0)
        {
            await Work.GetCollection<EmpDay>()
                .InsertOneAsync(empDay);
            return true;
        }

        return isExist.ModifiedCount > 0;
    }
}