using System.Linq.Expressions;
using MongoDB.Driver;
using SalaryCalculation.Data.BaseModels;

namespace SalaryCalculation.Data.Infrastructure;

public class UnitOfWork : IUnitOfWork
{
    protected readonly IMongoDatabase Database;

    public IMessageBroker MessageBroker { get; }

    public UnitOfWork(string connectionString, string databaseName, IMessageBroker broker) : this(new MongoUrl(connectionString), databaseName, broker)
    { }

    public UnitOfWork(MongoUrl connectionUrl, string databaseName, IMessageBroker broker)
    {
        MessageBroker = broker;
        var client = new MongoClient(connectionUrl);
        Database = client.GetDatabase(databaseName);
    }
    
    public IMongoCollection<T> GetCollection<T>()
    {
        return Database.GetCollection<T>(typeof(T).Name);
    }

    public IMongoCollection<T> GetCollection<T>(string name)
    {
        return Database.GetCollection<T>(name);
    }
    
    public async Task<K> NextValue<T, K>(int reservationCount = 1)
        where T : BaseMongoDomainModel
        where K : struct
    {
        var filter = Builders<Sequnce<K>>.Filter.Eq<string>((Expression<Func<Sequnce<K>, string>>) (x => x.Name), typeof (T).Name);
        var update = typeof (K) == typeof (long) ? Builders<Sequnce<K>>.Update.Inc<long>((FieldDefinition<Sequnce<K>, long>) "Value", (long) reservationCount) : Builders<Sequnce<K>>.Update.Inc<int>((FieldDefinition<Sequnce<K>, int>) "Value", reservationCount);
        var andUpdateOptions = new FindOneAndUpdateOptions<Sequnce<K>>();
        andUpdateOptions.IsUpsert = true;
        andUpdateOptions.ReturnDocument = ReturnDocument.After;
        var options = andUpdateOptions;
        return (await Database.GetCollection<Sequnce<K>>("sequnces").FindOneAndUpdateAsync<Sequnce<K>>(filter, update, (FindOneAndUpdateOptions<Sequnce<K>, Sequnce<K>>) options)).Value;
    }
}