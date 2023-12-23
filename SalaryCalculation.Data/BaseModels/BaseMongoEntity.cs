using MongoDB.Bson.Serialization.Attributes;

namespace SalaryCalculation.Data.BaseModels;

public class BaseMongoEntity<T>
{
    [BsonId]
    [BsonIgnoreIfDefault]
    public T Id { get; set; }
    public int Version { get; set; }

    public BaseMongoEntity()
    {
        Version = 1;
    }
}