using MongoDB.Bson.Serialization;
using MongoDB.Bson.Serialization.Attributes;

namespace SalaryCalculation.Data.BaseModels;

public abstract class BaseMongoDomainModel
{
    protected BaseMongoDomainModel() => this.Version = 1;

    public int Version { get; set; }
}

public class Sequnce<T> : BaseMongoDomainModel where T : struct
{
    public T Value { get; set; }

    [BsonId]
    public string Name { get; set; }
}

public class BaseMongoEntity<T> : BaseMongoDomainModel
{
    [BsonId]
    [BsonIgnoreIfDefault]
    public T Id { get; set; }
}

public class BaseMongoDomainModelMap : IDomainClassMap
{
    public BaseMongoDomainModelMap()
    {
        BsonClassMap.RegisterClassMap<BaseMongoDomainModel>(cm =>
        {
            cm.AutoMap();
            cm.GetMemberMap(c => c.Version).SetIgnoreIfDefault(true);
        });
    }
}

/// <summary>
/// Базовий інтерфейс профілю доменної моделі
/// </summary>
public interface IDomainClassMap
{
}