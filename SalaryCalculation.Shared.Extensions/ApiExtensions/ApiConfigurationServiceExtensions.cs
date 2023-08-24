using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.AspNetCore.Builder;
using Microsoft.Extensions.Logging;
using Organization.Data.Data;
using SalaryCalculation.Data;
using SalaryCalculation.Data.BaseHandlers;
using SalaryCalculation.Data.BaseModels;
using SalaryCalculation.Data.Infrastructure;
using SalaryCalculation.Data.Services;

namespace SalaryCalculation.Shared.Extensions.ApiExtensions;

public static class ApiConfigurationServiceExtensions
{
    public static IServiceCollection AddBusBackgroundService<TEvent, THandler>(this IServiceCollection services)
        where TEvent : BaseMessage
        where THandler : BaseMessageHandler<TEvent>
    {
        services.AddHostedService<BusBackgroundOrgService<TEvent, THandler>>();
        services.AddTransient<THandler>();
        return services;
    }
    
    public static IServiceCollection AddRabbitMessageBus(this IServiceCollection services, IConfiguration configuration)
    {
        var connectionString = configuration.GetValue<string>("Rabbit");
        return services.AddSingleton<IMessageBroker>(provider => new RabbitMqMessageBroker(connectionString));
    }
    
    public static IHostBuilder ConfigureSettings(this ConfigureHostBuilder builder, string fileName)
    {
        return builder.ConfigureAppConfiguration((hostingContext, config) =>
        {
            var env = hostingContext.HostingEnvironment;
            if (env.IsDevelopment())
            {
                var folder = AppDomain.CurrentDomain.BaseDirectory;
                config.AddJsonFile(Path.Combine(folder, $"{fileName}.json"), true);
                config.AddJsonFile(Path.Combine(folder, $"{fileName}.{env.EnvironmentName}.json"), true);
                config.AddJsonFile(Path.Combine(folder, $"{fileName}.secrets.json"), true);
                return;
            }

            config.AddJsonFile($"{fileName}.json", true);
            config.AddJsonFile($"{fileName}.{env.EnvironmentName}.json", true);
            config.AddJsonFile(Path.Combine("secrets", $"{fileName}.secrets.json"), true);
        });
    }
}

public class BusBackgroundOrgService<TEvent, THandler> : BusBackgroundService<TEvent, THandler>
    where TEvent : BaseMessage
    where THandler : BaseMessageHandler<TEvent>
{
    protected new IOrganizationUnitOfWork UnitOfWork;
    public BusBackgroundOrgService(IOrganizationUnitOfWork unitOfWork, IServiceScopeFactory serviceScopeFactory, ILogger<THandler> logger) : base(unitOfWork, serviceScopeFactory, logger)
    {
    }
}
