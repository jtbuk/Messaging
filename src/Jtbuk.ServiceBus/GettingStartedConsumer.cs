using MassTransit;

namespace Jtbuk.ServiceBus
{
    public class GettingStartedConsumer : IConsumer<InviteUser>
    {
        readonly ILogger<GettingStartedConsumer> _logger;

        public GettingStartedConsumer(ILogger<GettingStartedConsumer> logger)
        {
            _logger = logger;
        }

        public Task Consume(ConsumeContext<InviteUser> context)
        {
            _logger.LogInformation("Received Text: {Text}", context.Message.Value);
            return Task.CompletedTask;
        }
    }
}
