using System.Net.Http.Headers;
using Selise.Ecap.Infrastructure;

namespace Application.Common.Handlers;

public class TokenDelegatingHandler : DelegatingHandler
{
    private readonly ISecurityContextProvider _securityContextProvider;

    public TokenDelegatingHandler(ISecurityContextProvider securityContextProvider)
    {
        _securityContextProvider = securityContextProvider;
    }

    protected override async Task<HttpResponseMessage> SendAsync(
        HttpRequestMessage request,
        CancellationToken cancellationToken)
    {
        var token = _securityContextProvider.GetSecurityContext().OauthBearerToken;

        request.Headers.Authorization = new AuthenticationHeaderValue("bearer", token);

        return await base.SendAsync(request, cancellationToken);
    }
}