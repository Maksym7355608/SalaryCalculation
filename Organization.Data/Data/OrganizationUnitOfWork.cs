using MongoDB.Driver;
using SalaryCalculation.Data;
using SalaryCalculation.Data.Infrastructure;

namespace Organization.Data.Data;

public class OrganizationUnitOfWork : UnitOfWork, IOrganizationUnitOfWork
{
    public OrganizationUnitOfWork(string connectionString, string databaseName, IMessageBroker broker) : base(connectionString, databaseName, broker)
    {
    }

    public OrganizationUnitOfWork(MongoUrl connectionUrl, string databaseName, IMessageBroker broker) : base(connectionUrl, databaseName, broker)
    {
    }
}