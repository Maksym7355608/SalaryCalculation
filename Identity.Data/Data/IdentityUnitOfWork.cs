using MongoDB.Driver;
using SalaryCalculation.Data;
using SalaryCalculation.Data.Infrastructure;

namespace Identity.Data.Data;

public class IdentityUnitOfWork : UnitOfWork, IIdentityUnitOfWork
{
    public IdentityUnitOfWork(string connectionString, IMessageBroker broker) : base(connectionString, broker)
    {
    }

    public IdentityUnitOfWork(MongoUrl connectionUrl, IMessageBroker broker) : base(connectionUrl, broker)
    {
    }
}