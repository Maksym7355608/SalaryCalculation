using AutoMapper;
using Identity.App.Abstract;
using Identity.App.Handlers;
using Identity.Data.Data;
using SalaryCalculation.Data;
using SalaryCalculation.Data.Infrastructure;

namespace Identity.Api.CollectionExtensions;

public static class ServiceCollectionExtensions
{
    public static IServiceCollection AddMongoIdentityUnitOfWork(this IServiceCollection services, IConfiguration configuration)
    {
        var cs = configuration.GetValue<string>("MongoDbConnectionString");
        var dbName = configuration.GetValue<string>("IdentityDbName");
        return services.AddTransient(provider => {
            var bus = provider.GetService<IMessageBroker>();
            IIdentityUnitOfWork work = new IdentityUnitOfWork(cs, bus);

            return work;
        });
    }
    
    public static IServiceCollection AddRabbitMessageBus(this IServiceCollection services, IConfiguration configuration)
    {
        var connectionString = configuration.GetValue<string>("Rabbit");
        return services.AddSingleton<IMessageBroker>(provider => new RabbitMqMessageBroker(connectionString));
    }

    public static IServiceCollection AddCommandHandlers(this IServiceCollection services)
    {
        services.AddScoped<IIdentityCommandHandler, IdentityCommandHandler>();
        services.AddScoped<IRoleCommandHandler, RoleCommandHandler>();
        return services;
    }
}