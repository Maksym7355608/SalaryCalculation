namespace Dictionary.App.Dto;

public class FinanceDataDto
{
    public string Id { get; set; }
    public int Code { get; set; }
    public string Name { get; set; }
    /// <summary>
    /// 1 нарахування, -1 утримання
    /// </summary>
    public short Sign { get; set; }
    public int OrganizationId { get; set; }
    public string Description { get; set; }
}