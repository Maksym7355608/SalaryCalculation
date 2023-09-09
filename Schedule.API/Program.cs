using Microsoft.OpenApi.Models;
using SalaryCalculation.Shared.Extensions.ApiExtensions;
using Schedule.API;
using Schedule.App.Mapper;
using Serilog;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
builder.Host.ConfigureSettings("SharedSettings");

builder.Services.AddLogging(loggingBuilder => loggingBuilder.AddSerilog(dispose: true));
builder.Services.AddControllers();
// Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen(c =>
{
    c.SwaggerDoc("v1", new OpenApiInfo { Title = "Schedule API", Version = "v1" });
});

builder.Services.AddRabbitMessageBus(builder.Configuration);

builder.Services.AddMongoOrganizationUnitOfWork(builder.Configuration);
builder.Services.AddMongoScheduleUnitOfWork(builder.Configuration);
builder.Services.AddAutoMapper(typeof(ScheduleAutomapperProfile));
builder.Services.AddCommandHandlers();

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

app.UseAuthorization();

app.MapControllers();
app.UseRouting();
app.Run();