using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using Newtonsoft.Json;
using SalaryCalculation.Data.BaseHandlers;
using SalaryCalculation.Data.BaseModels;
using Serilog.Events;
using SerilogTimings;

namespace SalaryCalculation.Data.Services;

public abstract class BaseBusBackgroundService<TEvent, THandler> : BackgroundService
    where TEvent : 
    #nullable disable
    BaseMessage
    where THandler : BaseMessageHandler<TEvent>
  {
    private IServiceScopeFactory _serviceScopeFactory;

    protected virtual string SubscriptionId => this.GetType().Name;

    protected IUnitOfWork UnitOfWork { get; }

    protected ILogger<THandler> Logger { get; }

    protected BaseBusBackgroundService(
      IUnitOfWork unitOfWork,
      IServiceScopeFactory serviceScopeFactory,
      ILogger<THandler> logger)
    {
      UnitOfWork = unitOfWork;
      _serviceScopeFactory = serviceScopeFactory;
      Logger = logger;
    }

    protected override async Task ExecuteAsync(CancellationToken stoppingToken)
    {
      await UnitOfWork.MessageBroker.SubscribeAsync<TEvent>(SubscriptionId, HandleAsync);
    }

    protected virtual async Task HandleAsync(TEvent e)
    {
      using (Operation op = Operation.At(LogEventLevel.Debug).Begin("event {0}", (object)typeof(TEvent).Name))
      {
        using (IServiceScope scope = this.CreateScope())
        {
          try
          {
            this.LogEvent(e);
            await scope.ServiceProvider.GetRequiredService<THandler>().HandleAsync(e);
            op.Complete();
          }
          catch (Exception ex)
          {
            this.LogEventError(e, ex);
            throw;
          }
        }
      }
    }

    protected IServiceScope CreateScope() => this._serviceScopeFactory.CreateScope();

    protected virtual void LogEvent(TEvent msg)
    {
      if (this.Logger.IsEnabled(LogLevel.Trace))
      {
        this.Logger.LogTrace("Handled event {0}: {1}", (object) typeof (TEvent).Name, (object) JsonConvert.SerializeObject((object) msg));
      }
      else
      {
        if (!this.Logger.IsEnabled(LogLevel.Debug))
          return;
        this.Logger.LogDebug("Handled event {0}", (object) typeof (TEvent).Name);
      }
    }

    protected virtual void LogEventError(TEvent msg, Exception error) => this.LogEventError(msg, error.ToString());

    protected virtual void LogEventError(TEvent msg, string error) => this.Logger.LogError("Error event {0}: msg = {1}, error = {2}", (object) typeof (TEvent).Name, (object) JsonConvert.SerializeObject((object) msg), (object) error);
  }
  
public class BusBackgroundService<TEvent, THandler> : BaseBusBackgroundService<TEvent, THandler>
  where TEvent : BaseMessage
  where THandler : BaseMessageHandler<TEvent>
{
  public BusBackgroundService(
    IUnitOfWork unitOfWork,
    IServiceScopeFactory serviceScopeFactory,
    ILogger<THandler> logger)
    : base(unitOfWork, serviceScopeFactory, logger)
  {
  }
}