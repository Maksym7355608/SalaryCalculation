using Schedule.Data.Enums;

namespace Schedule.App.Dto;

public class EmpDayDto
{
    public EDayType DayType { get; set; }
    public DateTime Date { get; set; }
    public HoursDetailDto Hours { get; set; }
    public int EmployeeId { get; set; }
    public int OrganizationId { get; set; }
}