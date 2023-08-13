using Microsoft.OpenApi.Models;
using Organization.Api.CollectionExtensions;
using Organization.App.Commands.Messages;
using Organization.App.Handlers;
using Organization.App.Mapper;
using SalaryCalculation.Shared.Extensions.ApiExtensions;
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
    c.SwaggerDoc("v1", new OpenApiInfo { Title = "Organization API", Version = "v1" });
});

builder.Services.AddRabbitMessageBus(builder.Configuration);

builder.Services.AddMongoOrganizationUnitOfWork(builder.Configuration);
builder.Services.AddAutoMapper(typeof(OrganizationAutoMapperProfile));
builder.Services.AddCommandHandlers();

builder.Services.AddBusBackgroundService<EmployeeMassCreateMessage, EmployeeMassCreateMessageHandler>();
builder.Services.AddBusBackgroundService<EmployeeMassUpdateMessage, EmployeeMassUpdateMessageHandler>();
builder.Services.AddBusBackgroundService<EmployeeMassDeleteMessage, EmployeeMassDeleteMessageHandler>();

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