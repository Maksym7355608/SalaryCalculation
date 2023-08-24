namespace SalaryCalculation.Data.BaseModels;

public class IdNamePair
{
    public int Id { get; set; }
    public string Name { get; set; }

    public IdNamePair(int id, string name)
    {
        Id = id;
        Name = name;
    }
}