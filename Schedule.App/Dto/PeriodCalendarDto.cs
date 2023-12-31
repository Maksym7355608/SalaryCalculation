﻿using SalaryCalculation.Data.BaseModels;

namespace Schedule.App.Dto;

public class PeriodCalendarDto
{
    public int Period { get; set; }
    public int WorkDays { get; set; }
    public HoursDetailsDto Hours { get; set; }
    public int VacationDays { get; set; }
    public int SickLeave { get; set; }
    public int EmployeeId { get; set; }
    public int OrganizationId { get; set; }
    public IdNamePair Regime { get; set; }
}