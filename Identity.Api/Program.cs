using System.Text;
using Identity.Api.CollectionExtensions;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.IdentityModel.Tokens;
using Microsoft.OpenApi.Models;
using Serilog;
using Identity.App.Mapper;
using SalaryCalculation.Shared.Common.Attributes;
using SalaryCalculation.Shared.Extensions.ApiExtensions;
using Serilog.Events;

var builder = WebApplication.CreateBuilder(args);

builder.Host.ConfigureSettings("SharedSettings");
builder.Host.UseSerilog((hostingContext, loggerConfiguration) =>
    loggerConfiguration.ReadFrom.Configuration(hostingContext.Configuration));

Log.Logger = new LoggerConfiguration()
    .MinimumLevel.Debug()
    .MinimumLevel.Override("Microsoft", LogEventLevel.Information)
    .MinimumLevel.Override("System", LogEventLevel.Information)
    .Enrich.FromLogContext()
    .CreateLogger();

// Add services to the container.
builder.Services.AddLogging(loggingBuilder => loggingBuilder.AddSerilog(dispose: true));
builder.Services.AddControllers();
// Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
builder.Services.AddEndpointsApiExplorer();

builder.AddJwtAuthentication();
builder.Services.AddAllowCors();

builder.Services.AddAuthorization();
builder.Services.AddTransient<HandleExceptionAttribute>();
builder.Services.AddSwaggerGen(c =>
{
    c.SwaggerDoc("v1", new OpenApiInfo { Title = "Identity API", Version = "v1" });
});

builder.Services.AddRabbitMessageBus(builder.Configuration);

builder.Services.AddMongoIdentityUnitOfWork(builder.Configuration);
builder.Services.AddAutoMapper(typeof(IdentityAutoMapperProfile));
builder.Services.AddCommandHandlers();

builder.Services.AddCors(options =>
{
    options.AddPolicy(name: "ApiCorsPolicy",
        builder =>
        {
            builder.AllowAnyOrigin()
                .WithHeaders("Accept", "Content-Type")
                .AllowAnyMethod();
        });
});
builder.Services.AddCors(options =>
{
    options.AddPolicy(name: "ApiAuthorizedCorsPolicy",
        builder =>
        {
            builder.AllowAnyOrigin()
                .WithHeaders("Accept", "Content-Type", "Authorization")
                .AllowAnyMethod();
        });
});

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

app.UseRouting();
app.UseCors("ApiCorsPolicy");
app.UseCors("ApiAuthorizedCorsPolicy");
app.UseAuthentication();
app.UseAuthorization();
app.UseEndpoints(endpoints =>
{
    endpoints.MapControllers();
});
app.Run();