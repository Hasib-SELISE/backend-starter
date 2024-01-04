using Selise.Ecap.Infrastructure;

namespace Application.Common.Abstractions;

public interface ICommandDispatcher
{
    TResponse SendLocal<TCommand, TResponse>(TCommand command);
    Task<TResponse> SendLocalAsync<TCommand, TResponse>(TCommand command);
}