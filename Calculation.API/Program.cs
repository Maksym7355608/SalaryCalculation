using System.Text;
using Calculation.API;
using Calculation.App.Commands;
using Calculation.App.Handlers;
using Calculation.App.Mapper;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.IdentityModel.Tokens;
using Microsoft.OpenApi.Models;
using SalaryCalculation.Shared.Common.Attributes;
using SalaryCalculation.Shared.Extensions.ApiExtensions;
using Serilog;
using Serilog.Events;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
builder.Host.ConfigureSettings("SharedSettings");
builder.Host.UseSerilog((hostingContext, loggerConfiguration) =>
    loggerConfiguration.ReadFrom.Configuration(hostingContext.Configuration));

Log.Logger = new LoggerConfiguration()
    .MinimumLevel.Debug()
    .MinimumLevel.Override("Microsoft", LogEventLevel.Information)
    .MinimumLevel.Override("System", LogEventLevel.Information)
    .Enrich.FromLogContext()
    .CreateLogger();

builder.Services.AddLogging(loggingBuilder => loggingBuilder.AddSerilog(dispose: true));
builder.Services.AddControllers();

builder.AddJwtAuthentication();
builder.Services.AddAllowCors();
builder.Services.AddAuthorization();
builder.Services.AddTransient<HandleExceptionAttribute>();
// Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen(c =>
{
    c.SwaggerDoc("v1", new OpenApiInfo { Title = "Calculation API", Version = "v1" });
});

builder.Services.AddRabbitMessageBus(builder.Configuration);

builder.Services.AddMongoOrganizationUnitOfWork(builder.Configuration);
builder.Services.AddMongoScheduleUnitOfWork(builder.Configuration);
builder.Services.AddCalculationUnitOfWork(builder.Configuration);
builder.Services.AddAutoMapper(typeof(CalculationAutoMapperProfile));
builder.Services.AddCommandHandlers();

builder.Services.AddBusBackgroundService<CalculationSalaryMessage, CalculationSalaryMessageHandler>();
builder.Services.AddBusBackgroundService<MassCalculationMessage, MassCalculationSalaryMessageHandler>();

var app = builder.Build();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    
}
app.UseSwagger();
app.UseSwaggerUI(c =>
{
    c.SwaggerEndpoint("/swagger/v1/swagger.json", "Identity API v1");
});

app.UseAuthentication();
app.UseRouting();
app.UseAuthorization();
app.UseCors(options => options.AllowAnyOrigin().AllowAnyMethod().AllowAnyHeader());
app.UseEndpoints(endpoints =>
{
    endpoints.MapControllers();
});
app.Run();