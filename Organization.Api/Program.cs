using System.Text;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.IdentityModel.Tokens;
using Microsoft.OpenApi.Models;
using Organization.Api.CollectionExtensions;
using Organization.App.Commands.Messages;
using Organization.App.Handlers;
using Organization.App.Mapper;
using SalaryCalculation.Shared.Common.Attributes;
using SalaryCalculation.Shared.Extensions.ApiExtensions;
using Serilog;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
builder.Host.ConfigureSettings("SharedSettings");

builder.Services.AddLogging(loggingBuilder => loggingBuilder.AddSerilog(dispose: true));
builder.Services.AddControllers();

builder.Services.AddAuthentication(JwtBearerDefaults.AuthenticationScheme)
    .AddJwtBearer(options =>
    {
        options.TokenValidationParameters = new TokenValidationParameters
        {
            // Налаштування параметрів перевірки токена
            ValidateIssuer = true,
            ValidateAudience = true,
            ValidateLifetime = true,
            ValidIssuer = builder.Configuration["JwtSettings:Issuer"],
            ValidAudience = builder.Configuration["JwtSettings:Audience"],
            IssuerSigningKey =
                new SymmetricSecurityKey(Encoding.UTF8.GetBytes(builder.Configuration["JwtSettings:SecretKey"] ?? string.Empty)),
        };
    });
builder.Services.AddCors(options =>
{
    options.AddPolicy(name: "ApiAnonymousCorsPolicy",
        builder =>
        {
            builder.WithOrigins("http://localhost:3000", "https://localhost:3000")
                .AllowAnyHeader()
                .AllowAnyMethod()
                .AllowCredentials();
        });
});
builder.Services.AddCors(options =>
{
    options.AddPolicy(name: "ApiCorsPolicy",
        builder =>
        {
            builder.WithOrigins("http://localhost:3000", "https://localhost:3000")
                .WithHeaders("Content-Type", "Authorization")
                .AllowAnyMethod()
                .AllowCredentials();
        });
});
builder.Services.AddAuthorization();
builder.Services.AddTransient<HandleExceptionAttribute>();
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

app.UseAuthentication();
app.UseRouting();
app.UseAuthorization();
app.UseCors("ApiCorsPolicy");
app.UseCors("ApiAnonymousCorsPolicy");
app.UseEndpoints(endpoints =>
{
    endpoints.MapControllers();
});
app.Run();