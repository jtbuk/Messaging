using MassTransit;

namespace Jtbuk.ServiceBus.Features.MockApplications;

public static class SetupMockApplicationsFeature
{
    public static void AddMockApplicationsFeature(this WebApplication app)
    {

    }

    public static void RegisterMockConsumers(this IBusRegistrationConfigurator config)
    {
        config.AddConsumer<GettingStartedConsumer>();
    }
}
