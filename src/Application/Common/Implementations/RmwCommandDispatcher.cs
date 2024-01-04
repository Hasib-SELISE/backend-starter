using Application.Common.Abstractions;
using Selise.Ecap.Infrastructure;

namespace Application.Common.Implementations;

public class RmwCommandDispatcher : ICommandDispatcher
{
    private readonly CommandHandler _commandService;

    /// <summary>
    /// Base constructor 
    /// </summary>
    /// <param name="commandService"></param>
    public RmwCommandDispatcher(CommandHandler commandService)
    {
        _commandService = commandService;
    }

    /// <summary>
    /// Send to local 
    /// </summary>
    /// <param name="command"></param>
    /// <typeparam name="TCommand"></typeparam>
    /// <typeparam name="TResponse"></typeparam>
    /// <returns></returns>
    public TResponse SendLocal<TCommand, TResponse>(TCommand command)
    {
        return _commandService.Submit<TCommand, TResponse>(command);
    }

    /// <summary>
    /// Send to local async
    /// </summary>
    /// <param name="command"></param>
    /// <typeparam name="TCommand"></typeparam>
    /// <typeparam name="TResponse"></typeparam>
    /// <returns></returns>
    public Task<TResponse> SendLocalAsync<TCommand, TResponse>(TCommand command)
    {
        return _commandService.SubmitAsync<TCommand, TResponse>(command);
    }
}