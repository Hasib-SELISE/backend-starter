using Application.Common.Models.ResponseModels;

namespace Application.Common.Abstractions;

public interface IHttpRequestGateway
{
    Task<HttpRequestResponse<TResponse>> MakeRequestAsync<TResponse>(HttpClient client, HttpRequestMessage httpRequestMessage, IdentityTokenResponse authorizationToken = null) where TResponse : class, new();
}