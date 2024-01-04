using Selise.Ecap.Hosting.RabbitMq;
using Selise.Ecap.Infrastructure;

namespace WebHost.ServiceExtensions;

public static class AmpqServiceRegistration
{
    public static AmpqMessageConsumerOptions AddMessageConsumerOptions(IAppSettings appSettings)
    {
        var ampqMessageConsumerOptions = new AmpqMessageConsumerOptions();

        /*ampqMessageConsumerOptions
            .ConsumerSubscriptions
            .ListenOn(ServiceBusMessageConstants.CommandQueueName, 1)*/


        return ampqMessageConsumerOptions;
    }

    public static string[] AddRequiredQueues(IAppSettings appSettings)
    {
        return new[]
        {
            appSettings.EcapAuditLogQueueName,
            //ServiceBusMessageConstants.CommandQueueName,
        };
    }
}