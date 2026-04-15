using System.Reflection;
using Microsoft.EntityFrameworkCore;
using MongoDB.Driver;
using System.Runtime.InteropServices;
using System.Text.Json.Serialization;
using FluentValidation;
using FluentValidation.AspNetCore;
using Claims.Application.Factories;
using Claims.Application.Interfaces;
using Claims.Application.Services;
using Claims.Domain.Entities;
using Claims.Domain.Interfaces;
using Claims.Infrastructure.Auditing;
using Claims.Infrastructure.Database;
using Claims.Infrastructure.Repositories;
using Claims.API.Middleware;
using Testcontainers.MongoDb;
using Testcontainers.MsSql;

var builder = WebApplication.CreateBuilder(args);

// Start Testcontainers for SQL Server and MongoDB
var sqlContainer = (RuntimeInformation.IsOSPlatform(OSPlatform.Linux)
        ? new MsSqlBuilder().WithImage("mcr.microsoft.com/mssql/server:2022-latest")
        : new()
    ).Build();

var mongoContainer = new MongoDbBuilder()
    .WithImage("mongo:latest")
    .Build();

await sqlContainer.StartAsync();
await mongoContainer.StartAsync();

// Add services to the container.
builder.Services
    .AddControllers()
    .AddJsonOptions(x =>
    {
        x.JsonSerializerOptions.Converters.Add(new JsonStringEnumConverter());
    });

builder.Services.AddFluentValidationAutoValidation();

// Auto-validation 
builder.Services.AddValidatorsFromAssemblyContaining<Program>(
    ServiceLifetime.Scoped,
    filter =>
        filter.ValidatorType.Namespace != null &&
        filter.ValidatorType.Namespace.StartsWith("Claims.API.Validators"));

// manual validation
builder.Services.AddValidatorsFromAssemblyContaining<Claims.Application.Validators.ClaimValidator>(
    ServiceLifetime.Scoped,
    filter =>
        filter.ValidatorType.Namespace != null &&
        filter.ValidatorType.Namespace.StartsWith("Claims.Application.Validators"));

builder.Services.AddProblemDetails();
builder.Services.AddExceptionHandler<GlobalExceptionHandler>();

builder.Services.AddDbContext<AuditContext>(options =>
    options.UseSqlServer(sqlContainer.GetConnectionString()));

builder.Services.AddDbContext<ClaimsContext>(options =>
{
    var client = new MongoClient(mongoContainer.GetConnectionString());
    var database = client.GetDatabase(builder.Configuration["MongoDb:DatabaseName"]); // Use a default/test database name
    options.UseMongoDB(database.Client, database.DatabaseNamespace.DatabaseName);
});

// Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen(options =>
{
    options.SwaggerDoc("v1", new() { Title = "Claims.Api", Version = "v1", Description = "Solution to solve Tasks for the technical interview by Matviiv Victor"});
    options.UseInlineDefinitionsForEnums();
    
    var xmlFile = $"{Assembly.GetExecutingAssembly().GetName().Name}.xml";
    var xmlPath = Path.Combine(AppContext.BaseDirectory, xmlFile);
    options.IncludeXmlComments(xmlPath);
});

builder.Services.AddScoped<IClaimsRepository, ClaimsRepository>();
builder.Services.AddScoped<IClaimsService, ClaimsService>();
builder.Services.AddScoped<IAuditer, Auditer>();
builder.Services.AddSingleton<IAuditQueue, AuditQueue>();
builder.Services.AddHostedService<AuditBackgroundService>();
builder.Services.AddScoped<ICoversRepository, CoversRepository>();
builder.Services.AddScoped<ICoversService, CoversService>();
builder.Services.AddTransient<IPremiumService, PremiumService>();
builder.Services.AddScoped<Func<CoverType, ICoverTypeComputePremiumStategy>>(_ => MultiplierStrategyFactory.GetStrategy);

var app = builder.Build();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseExceptionHandler();

app.UseHttpsRedirection();

app.UseAuthorization();

app.MapControllers();

using (var scope = app.Services.CreateScope())
{
    var context = scope.ServiceProvider.GetRequiredService<AuditContext>();
    context.Database.Migrate();
}

app.Run();

public partial class Program { }