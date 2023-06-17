using MongoDB.Driver;
using SalaryCalculation.Data.BaseEventModels;

namespace SalaryCalculation.Data;

public interface IUnitOfWork
{
    IMongoCollection<T> GetCollection<T>();
    IMongoCollection<T> GetCollection<T>(string name);
    Task PublishEventAsync<M>(M msg) where M : BusEvent;
    Task PublishEventAsync<TBase, T>(T msg)
        where TBase : BusEvent
        where T : TBase;
}