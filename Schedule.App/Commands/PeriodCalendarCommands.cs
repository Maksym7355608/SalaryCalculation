using SalaryCalculation.Data.BaseModels;

namespace Schedule.App.Commands;

public class PeriodCalendarSearchCommand : BaseCommand
{
    public int PeriodFrom { get; set; }
    public int? PeriodTo { get; set; }
    public int? VacationDays { get; set; }
    public int? SickLeave { get; set; }
    public long? EmployeeNumber { get; set; }
    public int OrganizationId { get; set; }
    public int? RegimeId { get; set; }
}

public class PeriodCalendarMassCalculateCommand : BaseCommand
{
    public int PeriodFrom { get; set; }
    public int? PeriodTo { get; set; }
    public int OrganizationId { get; set; }
    public int[] RegimeIds { get; set; }
}

public class PeriodCalendarMassCalculateMessage : BaseMessage
{
    public int PeriodFrom { get; set; }
    public int? PeriodTo { get; set; }
    public int OrganizationId { get; set; }
    public int[] RegimeIds { get; set; }
}