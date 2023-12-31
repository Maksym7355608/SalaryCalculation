﻿using SalaryCalculation.Data.BaseModels;
using Schedule.App.Commands;
using Schedule.App.Dto;

namespace Schedule.App.Abstract;

public interface IScheduleCommandHandler
{
    Task<IEnumerable<RegimeDto>> GetRegimesAsync(int organizationId);
    Task<RegimeDto> GetRegimeAsync(int id);
    Task<bool> CreateRegimeAsync(RegimeCreateCommand command);
    Task<bool> UpdateRegimeAsync(RegimeUpdateCommand command);
    Task<bool> DeleteRegimeAsync(int id);

    Task<IEnumerable<WorkDayDetailDto>> GetWorkDaysRegimeAsync(int regimeId);
    Task<bool> UpdateWorkDayRegimeAsync(WorkDayRegimeUpdateCommand command);

    Task<IEnumerable<PeriodCalendarDto>> GetPeriodsCalendarByEmployeeAsync(int employeeId);
    Task<IEnumerable<PeriodCalendarDto>> SearchPeriodsCalendarAsync(PeriodCalendarSearchCommand command);
    Task<PeriodCalendarDto> GetPeriodCalendarAsync(int employeeId, int period);

    Task<IEnumerable<EmployeeScheduleDto>> GetWorkDaysAsync(WorkDaySearchCommand command);
    Task<IEnumerable<EmpDayDto>> GetEmpDaysByEmployeeAsync(int employeeId, int period);
    Task<bool> SetWorkDayAsync(WorkDaysUpdateCommand command);
    
    Task<bool> CalculatePeriodCalendarAsync(int employeeId, int period);
    Task MassCalculatePeriodCalendarAsync(PeriodCalendarMassCalculateCommand command);
    Task QuickSettingDaysAsync(DaysSettingMessage msg);
    Task<IEnumerable<IdNamePair>> GetRegimesShortAsync(int organizationId);
}