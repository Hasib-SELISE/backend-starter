using Application.Common.Abstractions;
using Selise.Ecap.Infrastructure;

namespace Application.Common.Implementations;

public class MessageDispatcher : IMessageDispatcher
{
    private readonly IServiceClient _serviceClient;

    /// <summary>
    /// Base constructor
    /// </summary>
    /// <param name="serviceClient"></param>
    public MessageDispatcher(IServiceClient serviceClient)
    {
        _serviceClient = serviceClient;
    }
    
    /// <summary>
    /// Send to queue
    /// </summary>
    /// <param name="queueName"></param>
    /// <param name="payload"></param>
    /// <typeparam name="TResponse"></typeparam>
    /// <returns></returns>
    public TResponse SendToQueue<TResponse>(string queueName, object payload) where TResponse : new()
    {
        return _serviceClient.SendToQueue<TResponse>(queueName, payload);
    }

    /// <summary>
    /// Send to queue
    /// </summary>
    /// <param name="queueName"></param>
    /// <param name="payload"></param>
    /// <param name="numberOfRetryAttempts"></param>
    /// <param name="iterationDelay"></param>
    /// <param name="keepInAFailQueue"></param>
    /// <typeparam name="TResponse"></typeparam>
    /// <returns></returns>
    public TResponse SendToQueue<TResponse>(string queueName, object payload, int numberOfRetryAttempts,
        TimeSpan iterationDelay, bool keepInAFailQueue = false) where TResponse : new()
    {
        return _serviceClient.SendToQueue<TResponse>(queueName, payload, numberOfRetryAttempts, iterationDelay,
            keepInAFailQueue);
    }

    /// <summary>
    /// Send to queue
    /// </summary>
    /// <param name="queueName"></param>
    /// <param name="payload"></param>
    /// <param name="securityContext"></param>
    /// <typeparam name="TResponse"></typeparam>
    /// <returns></returns>
    public TResponse SendToQueue<TResponse>(string queueName, object payload, SecurityContext securityContext)
        where TResponse : new()
    {
        return _serviceClient.SendToQueue<TResponse>(queueName, payload, securityContext);
    }

    public TResponse SendToQueue<TResponse>(string queueName, object payload, SecurityContext securityContext,
        int numberOfRetryAttempts, TimeSpan iterationDelay, bool keepInAFailQueue = false)
        where TResponse : new()
    {
        return _serviceClient.SendToQueue<TResponse>(queueName, payload, securityContext, numberOfRetryAttempts,
            iterationDelay, keepInAFailQueue);
    }
}