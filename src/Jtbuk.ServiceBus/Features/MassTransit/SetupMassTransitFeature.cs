using Jtbuk.ServiceBus.Features.MockApplications;
using MassTransit;

namespace Jtbuk.ServiceBus.Features.Applications;

public static class SetupMassTransitFeature
{
    public static void AddMassTransitSetup(this WebApplicationBuilder builder)
    {
        var services = builder.Services;
        var configuration = builder.Configuration;

        services.AddMassTransit(o =>
        {
            o.RegisterMockConsumers();
            o.UsingInMemory((context, cfg) =>
            {
                cfg.ConfigureEndpoints(context);
            });
            //o.UsingAzureServiceBus((context, cfg) =>
            //{
            //    cfg.Host(configuration.GetConnectionString("AzureServiceBus"));
            //    cfg.ConfigureEndpoints(context);
            //});
            o.SetKebabCaseEndpointNameFormatter();
        });

        services.AddOptions<MassTransitHostOptions>()
        .Configure(options =>
        {
            options.WaitUntilStarted = true;
            options.StartTimeout = TimeSpan.FromSeconds(10);
            options.StopTimeout = TimeSpan.FromSeconds(30);
        });
    }
}

