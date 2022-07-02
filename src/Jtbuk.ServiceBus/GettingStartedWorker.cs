using MassTransit;

namespace Jtbuk.ServiceBus;

public class GettingStartedWorker : BackgroundService
{
    readonly IBus _bus;

    public GettingStartedWorker(IBus bus)
    {
        _bus = bus;
    }

    protected async override Task ExecuteAsync(CancellationToken stoppingToken)
    {
        while (!stoppingToken.IsCancellationRequested)
        {
            await _bus.Publish(new InviteUser($"The time is {DateTimeOffset.Now}"), stoppingToken);

            await Task.Delay(1000, stoppingToken);
        }
    }
}
