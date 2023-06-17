using MongoDB.Driver;
using SalaryCalculation.Data.BaseEventModels;

namespace SalaryCalculation.Data;

public class UnitOfWork : IUnitOfWork
{
    private readonly IMongoDatabase _database;
    private readonly IMessageBus _bus;

    public UnitOfWork(IMongoDatabase database, IMessageBus bus)
    {
        _database = database;
        _bus = bus;
    }

    public IMongoCollection<T> GetCollection<T>()
    {
        return _database.GetCollection<T>(typeof(T).Name);
    }

    public IMongoCollection<T> GetCollection<T>(string collectionName)
    {
        return _database.GetCollection<T>(collectionName);
    }

    public Task PublishEventAsync<M>(M msg) where M : BusEvent
    {
        return _bus.PublishEventAsync(msg);
    }

    public Task PublishEventAsync<TBase, T>(T msg)
        where TBase : BusEvent
        where T : TBase
    {
        return _bus.PublishEventAsync<TBase, T>(msg);
    }
}