using MongoDB.Driver;
using SalaryCalculation.Data;
using SalaryCalculation.Data.Infrastructure;

namespace Identity.Data.Data;

public class IdentityUnitOfWork : UnitOfWork, IIdentityUnitOfWork
{
    public IdentityUnitOfWork(string connectionString, string databaseName, IMessageBroker broker) : base(connectionString, databaseName, broker)
    {
    }

    public IdentityUnitOfWork(MongoUrl connectionUrl, string databaseName, IMessageBroker broker) : base(connectionUrl, databaseName, broker)
    {
    }
}