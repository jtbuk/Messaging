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

app.AddSwaggerFeature();
app.UseHttpsRedirection();
app.AddApplicationsFeature();
app.AddUsersFeature();
app.AddTenantsFeature();
app.AddEntitlementFeature();
app.Run();

public partial class Program { }