using Jtbuk.ServiceBus.Features.Entitlements;
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
            o.UsingAzureServiceBus((context, cfg) =>
            {
                //https://masstransit-project.com/advanced/topology/servicebus.html#azure-service-bus
                cfg.Host(configuration.GetConnectionString("AzureServiceBus"));
                cfg.ConfigureEndpoints(context);

                //https://docs.microsoft.com/en-us/azure/service-bus-messaging/service-bus-partitioning                

                //Publish = Publishing an event
                //https://masstransit-project.com/usage/producers.html#send
                //cfg.Publish<SetEntitlementDto>(topology =>
                //{
                //    topology.Use
                //});

                //Send = Sending a command
                //https://masstransit-project.com/usage/producers.html#send
                cfg.Send<SetEntitlementDto>(topology => {                    
                    topology.UseCorrelationId((dto) => dto.ApplicationUniqueId);
                });
                
            });
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

