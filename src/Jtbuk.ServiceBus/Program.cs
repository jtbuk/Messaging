using Jtbuk.ServiceBus;
using Jtbuk.ServiceBus.Data;
using Jtbuk.ServiceBus.Features.Applications;
using Jtbuk.ServiceBus.Features.Entitlements;
using Jtbuk.ServiceBus.Features.Swagger;
using Jtbuk.ServiceBus.Features.Tenants;
using Jtbuk.ServiceBus.Features.Users;
using Microsoft.EntityFrameworkCore;

var builder = WebApplication.CreateBuilder(args);
builder.AddMassTransitSetup();

var services = builder.Services;
var configuration = builder.Configuration;

//services.AddDbContext<DatabaseContext>(options => options.UseSqlServer(configuration.GetConnectionString("Database")));
services.AddDbContext<DatabaseContext>(options => options.UseInMemoryDatabase("Memory"));
services.AddEndpointsApiExplorer();
services.UseSwaggerFeature();

var app = builder.Build();

app.AddSwaggerFeature();
app.UseHttpsRedirection();
app.AddApplicationsFeature();
app.AddUsersFeature();
app.AddTenantsFeature();
app.AddEntitlementFeature();
app.Run();

public partial class Program { }