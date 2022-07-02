using MassTransit;
using MassTransit.Testing;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using System;
using System.IO;
using System.Reflection;

namespace Jtbuk.IntegrationTests
{
    public class BaseTest
    {
        public IConfiguration Configuration => new ConfigurationBuilder()
                .SetBasePath(Directory.GetCurrentDirectory())
                .AddJsonFile("appsettings.json", false, false)
                .AddEnvironmentVariables()
                .AddUserSecrets(Assembly.GetExecutingAssembly())
                .Build();

        public ITestHarness GetTestHarness(
            Action<IBusRegistrationConfigurator>? addConfiguration = null,
            Action<IServiceBusBusFactoryConfigurator>? addBusConfiguration = null)
        {
            var provider = new ServiceCollection()
            .AddMassTransitTestHarness(o =>
            {                
                o.UsingAzureServiceBus((context, cfg) =>
                {
                    cfg.Host(Configuration.GetConnectionString("AzureServiceBus"));
                    cfg.ConfigureEndpoints(context);

                    addBusConfiguration?.Invoke(cfg);
                });
                o.SetKebabCaseEndpointNameFormatter();
                addConfiguration?.Invoke(o);
            })
            .BuildServiceProvider(true);

            return provider.GetRequiredService<ITestHarness>();
        }
    }
}
