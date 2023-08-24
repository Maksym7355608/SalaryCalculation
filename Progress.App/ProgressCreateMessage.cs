using SalaryCalculation.Data.BaseModels;

namespace Progress.App;

public class ProgressCreateMessage : BaseMessage
{
    /// <summary>
    /// Guid
    /// </summary>
    public string ProgressId { get; set; }
}