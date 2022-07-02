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
        //o.UsingInMemory((context, cfg) =>
        //{
        //    cfg.ConfigureEndpoints(context);
        //});        
            o.UsingAzureServiceBus((context, cfg) =>
            {
                //https://masstransit-project.com/advanced/topology/servicebus.html#azure-service-bus
                cfg.Host(configuration.GetConnectionString("AzureServiceBus"));
                cfg.ConfigureEndpoints(context);

                //https://docs.microsoft.com/en-us/azure/service-bus-messaging/service-bus-partitioning
                cfg.EnablePartitioning = true;

                //Publish = Publishing an event
                //https://masstransit-project.com/usage/producers.html#send
                //cfg.Publish<SetEntitlementDto>(topology =>
                //{
                //    //topology.
                //});

                //Send = Sending a command
                //https://masstransit-project.com/usage/producers.html#send
                cfg.Send<SetEntitlementDto>(topology =>
                {
                    topology.UseCorrelationId((dto) => dto.ApplicationUniqueId);
                    topology.UseSessionIdFormatter(context => context.Message.UserUniqueId.ToString());
                    //topology.UsePartitionKeyFormatter(context => context.Message.UserUniqueId.ToString());
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

