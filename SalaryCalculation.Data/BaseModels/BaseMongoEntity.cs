namespace SalaryCalculation.Data.BaseModels;

public class BaseMongoEntity
{
    public int Version { get; set; }

    public BaseMongoEntity()
    {
        Version = 1;
    }
}