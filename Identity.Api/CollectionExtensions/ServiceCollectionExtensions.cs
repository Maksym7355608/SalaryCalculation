using AutoMapper;
using Identity.App.Abstract;
using Identity.App.Handlers;
using Identity.Data.Data;
using SalaryCalculation.Data;
using SalaryCalculation.Data.Infrastructure;
using SalaryCalculation.Shared.Extensions.ApiExtensions;

namespace Identity.Api.CollectionExtensions;

public static class ServiceCollectionExtensions
{
    public static IServiceCollection AddMongoIdentityUnitOfWork(this IServiceCollection services, IConfiguration configuration)
    {
        var cs = configuration.GetValue<string>("MongoDbConnectionString");
        var dbName = configuration.GetValue<string>("IdentityDbName");
        return services.AddTransient(provider => {
            var bus = provider.GetService<IMessageBroker>();
            IIdentityUnitOfWork work = new IdentityUnitOfWork(cs, dbName, bus);

            return work;
        });
    }
    
    public static IServiceCollection AddCommandHandlers(this IServiceCollection services)
    {
        services.AddScoped<IIdentityCommandHandler, IdentityCommandHandler>();
        services.AddScoped<IRoleCommandHandler, RoleCommandHandler>();
        return services;
    }
}