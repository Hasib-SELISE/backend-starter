using Selise.Ecap.Infrastructure;

namespace Application.Common.Handlers;

public abstract class AbstractCommandHandler<TCommand, TResponse>
    : ICommandHandler<TCommand, TResponse> where TCommand : class
{
    public TResponse Handle(TCommand command)
    {
        return HandleAsync(command).Result;
    }

    public abstract Task<TResponse> HandleAsync(TCommand command);
}