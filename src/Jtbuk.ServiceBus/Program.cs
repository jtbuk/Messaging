using Jtbuk.ServiceBus.Data;
using Jtbuk.ServiceBus.Features.Applications;
using Jtbuk.ServiceBus.Features.Entitlements;
using Jtbuk.ServiceBus.Features.Swagger;
using Jtbuk.ServiceBus.Features.Tenants;
using Jtbuk.ServiceBus.Features.Users;
using MassTransit;
using Microsoft.ApplicationInsights;
using Microsoft.ApplicationInsights.DependencyCollector;
using Microsoft.ApplicationInsights.Extensibility;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging.ApplicationInsights;
using Microsoft.Extensions.Options;

var builder = WebApplication.CreateBuilder(args);
builder.AddMassTransitSetup();

var services = builder.Services;
var configuration = builder.Configuration;

services.AddDbContext<DatabaseContext>(options => options.UseSqlServer(configuration.GetConnectionString("Database")));
//services.AddDbContext<DatabaseContext>(options => options.UseInMemoryDatabase("Memory"));
services.AddEndpointsApiExplorer();
services.UseSwaggerFeature();

var app = builder.Build();

using (var scope = app.Services.CreateScope())
{
    var context = scope.ServiceProvider.GetRequiredService<DatabaseContext>();
    context.Database.EnsureDeleted();
    context.Database.EnsureCreated();
}

//Dunno how to clean up this logging...
//var module = new DependencyTrackingTelemetryModule();
//module.IncludeDiagnosticSourceActivities.Add("MassTransit");

//TelemetryConfiguration telemetryConfiguration = TelemetryConfiguration.CreateDefault();
//telemetryConfiguration.InstrumentationKey = "<your instrumentation key>";
//telemetryConfiguration.TelemetryInitializers.Add(new HttpDependenciesParsingTelemetryInitializer());

//var telemetryClient = new TelemetryClient(telemetryConfiguration);
//module.Initialize(telemetryConfiguration);

//var loggerOptions = new ApplicationInsightsLoggerOptions();

//var applicationInsightsLoggerProvider = new ApplicationInsightsLoggerProvider(
//    Options.Create(telemetryConfiguration),
//    Options.Create(loggerOptions));

//ILoggerFactory factory = new LoggerFactory();
//factory.AddProvider(applicationInsightsLoggerProvider);

//LogContext.ConfigureCurrentLogContext(factory);

app.AddSwaggerFeature();
app.UseHttpsRedirection();
app.AddApplicationsFeature();
app.AddUsersFeature();
app.AddTenantsFeature();
app.AddEntitlementFeature();
app.Run();

telemetryClient.Flush();

public partial class Program { }