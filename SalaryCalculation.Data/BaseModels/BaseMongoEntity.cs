namespace SalaryCalculation.Data.BaseModels;

public class BaseMongoEntity<T>
{
    public T Id { get; set; }
    public int Version { get; set; }

    public BaseMongoEntity()
    {
        Version = 1;
    }
}