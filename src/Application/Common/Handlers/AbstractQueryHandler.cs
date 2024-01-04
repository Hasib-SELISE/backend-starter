using Selise.Ecap.Infrastructure;

namespace Application.Common.Handlers;

public abstract class AbstractQueryHandler<TQuery, TResponse> : IQueryHandler<TQuery, TResponse>
{
    public TResponse Handle(TQuery query)
    {
        return HandleAsync(query).Result;
    }

    public abstract Task<TResponse> HandleAsync(TQuery query);
}