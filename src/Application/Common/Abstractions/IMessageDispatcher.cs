using Selise.Ecap.Infrastructure;

namespace Application.Common.Abstractions;

public interface IMessageDispatcher
{
    TResponse SendToQueue<TResponse>(string queueName, object payload) where TResponse : new();

    TResponse SendToQueue<TResponse>(string queueName, object payload, int numberOfRetryAttempts,
        TimeSpan iterationDelay, bool keepInAFailQueue = false) where TResponse : new();

    TResponse SendToQueue<TResponse>(string queueName, object payload, SecurityContext securityContext)
        where TResponse : new();

    TResponse SendToQueue<TResponse>(string queueName, object payload, SecurityContext securityContext,
        int numberOfRetryAttempts, TimeSpan iterationDelay, bool keepInAFailQueue = false)
        where TResponse : new();
}