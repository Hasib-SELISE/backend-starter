using Application.Common.Abstractions;
using Selise.Ecap.Infrastructure;

namespace Application.Common.Implementations;

public class QueryDispatcher : IQueryDispatcher
{
    private readonly QueryHandler _queryHandler;
    private readonly IRmwLogger<QueryDispatcher> _logger;

    public QueryDispatcher(QueryHandler queryHandler, IServiceClient serviceClient
        , IRmwLogger<QueryDispatcher> logger)
    {
        _queryHandler = queryHandler;
        _logger = logger;
    }

    public TResponse Submit<TCommand, TResponse>(TCommand command)
    {
        return _queryHandler.Submit<TCommand, TResponse>(command);
    }

    public Task<TResponse> SubmitAsync<TCommand, TResponse>(TCommand command)
    {
        return _queryHandler.SubmitAsync<TCommand, TResponse>(command);
    }
}