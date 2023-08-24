using Organization.App.Abstract;
using Organization.App.Handlers;
using Organization.Data.Data;
using SalaryCalculation.Data;

namespace Organization.Api.CollectionExtensions;

public static class ServiceCollectionExtensions
{
    public static IServiceCollection AddMongoOrganizationUnitOfWork(this IServiceCollection services, IConfiguration configuration)
    {
        var cs = configuration.GetValue<string>("MongoDbConnectionString");
        var dbName = configuration.GetValue<string>("OrganizationDbName");
        return services.AddTransient(provider => {
            var bus = provider.GetService<IMessageBroker>();
            IOrganizationUnitOfWork work = new OrganizationUnitOfWork(cs, dbName, bus);

            return work;
        });
    }
    
    public static IServiceCollection AddCommandHandlers(this IServiceCollection services)
    {
        services.AddScoped<IOrganizationCommandHandler, OrganizationCommandHandler>();
        services.AddScoped<IEmployeeCommandHandler, EmployeeCommandHandler>();
        services.AddScoped<IManagerCommandHandler, ManagerCommandHandler>();
        return services;
    }
}