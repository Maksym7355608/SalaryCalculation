using SalaryCalculation.Data.BaseModels;
using Schedule.App.Dto;
using Schedule.Data.Enums;

namespace Schedule.App.Commands;

public class WorkDayCreateCommand : BaseCommand
{
    public EDayType DayType { get; set; }
    public DateTime Date { get; set; }
    public HoursDetailDto Hours { get; set; }
    public int EmployeeId { get; set; }
    public int OrganizationId { get; set; }
}

public class WorkDaySearchCommand : BaseCommand
{
    public int Period { get; set; }
    public int[]? OrganizationUnitIds { get; set; }
    public int[]? PositionIds { get; set; }
    public int OrganizationId { get; set; }
}

public class DaysSettingMessage : BaseMessage
{
    public DateTime DateFrom { get; set; }
    public DateTime DateTo { get; set; }
    
    public int OrganizationId { get; set; }
    public int? OrganizationUnitId { get; set; }
    public int? PositionId { get; set; }
    public int? RegimeId { get; set; }
}