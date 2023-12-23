namespace Schedule.App.Dto;

public class EmployeeScheduleDto
{
    public int Id { get; set; }
    public string Name { get; set; }
    public int Period { get; set; }
    public IEnumerable<EmpDayShortDto> Schedule { get; set; }
}