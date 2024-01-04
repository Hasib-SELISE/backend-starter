namespace Application.Common.Abstractions;

public interface IQueryDispatcher
{
    public TQueryResponse Submit<TQuery, TQueryResponse>(TQuery query);
    public Task<TQueryResponse> SubmitAsync<TQuery, TQueryResponse>(TQuery query);
}