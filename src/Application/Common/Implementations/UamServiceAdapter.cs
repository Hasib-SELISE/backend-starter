using Application.Common.Abstractions;
using Application.Common.Enums;
using Application.Common.Models;
using Application.Common.Models.ResponseModels;

namespace Application.Common.Implementations;

public class UamServiceAdapter : IUamServiceAdapter
{
    private readonly HttpClient _httpClient;
    private readonly IIdentityServiceAdapter _identityServiceConsumer;
    private readonly HttpRequestHelper _httpRequestHelper;
    private readonly IHttpRequestGateway _requestGateway;
    private readonly IRmwSettings _rmwSettings;
    private readonly IRmwLogger<UamServiceAdapter> _logger;

    public UamServiceAdapter(
        HttpClient httpClient,
        IIdentityServiceAdapter identityServiceConsumer,
        HttpRequestHelper httpRequestHelper,
        IHttpRequestGateway requestGateway,
        IRmwSettings rmwSettings,
        IRmwLogger<UamServiceAdapter> logger)
    {
        _httpClient = httpClient;
        _identityServiceConsumer = identityServiceConsumer;
        _httpRequestHelper = httpRequestHelper;
        _requestGateway = requestGateway;
        _rmwSettings = rmwSettings;
        _logger = logger;
    }
    
    /// <summary>
    /// Create user 
    /// </summary>
    /// <param name="user"></param>
    /// <param name="token"></param>
    /// <returns></returns>
    public async Task<HttpRequestResponse<EcapServiceResponse>> CreateUserAsync(UserCreationModel user, string token)
    {
        var tokenResponse = string.IsNullOrWhiteSpace(token)
            ? await _identityServiceConsumer.GetTokenAsync(EcapCredentialType.Client)
            : new IdentityTokenResponse
            {
                AccessToken = token,
                TokenType = "Bearer"
            };

        var uam = _rmwSettings.EcapServices.Services.FindLast(x => x.ServiceName == EcapServiceType.Uam);
        
        var uri = $"{_httpClient.BaseAddress}{uam?.ServiceVersion}{uam?.ServiceEndpoint["CreateUser"]}";
        
        var httpRequestMessage = _httpRequestHelper.PrepareHttpRequest(uri, HttpMethod.Post, user);
        
        var response = await _requestGateway.MakeRequestAsync<EcapServiceResponse>(_httpClient, httpRequestMessage, tokenResponse);
        
        return response;
    }

    /// <summary>
    /// Update user 
    /// </summary>
    /// <param name="user"></param>
    /// <param name="token"></param>
    /// <returns></returns>
    public async Task<HttpRequestResponse<EcapServiceResponse>> UpdateUserAsync(UserUpdateModel user, string token)
    {
        var tokenResponse = string.IsNullOrWhiteSpace(token)
            ? await _identityServiceConsumer.GetTokenAsync(EcapCredentialType.Client)
            : new IdentityTokenResponse
            {
                AccessToken = token,
                TokenType = "Bearer"
            };

        var uam = _rmwSettings.EcapServices.Services.FindLast(x => x.ServiceName == EcapServiceType.Uam);
        
        var credential = _rmwSettings.EcapCredentials.Credentials.FindLast(x => x.CredentialType == EcapCredentialType.Client);
        
        var uri = $"{_httpClient.BaseAddress}{uam.ServiceVersion}{uam.ServiceEndpoint["UpdateUser"]}";
        
        var httpRequestMessage = _httpRequestHelper.PrepareHttpRequest(uri, HttpMethod.Post, user);
        
        var response = await _requestGateway.MakeRequestAsync<EcapServiceResponse>(_httpClient, httpRequestMessage, tokenResponse);
        
        return response;
    }

    public async Task<HttpRequestResponse<EcapServiceResponse>> DeleteProfilePermanentlyAsync(UserDeletionModel userDeletionModel, string token)
    {
        var tokenResponse = string.IsNullOrWhiteSpace(token)
            ? await _identityServiceConsumer.GetTokenAsync(EcapCredentialType.Client)
            : new IdentityTokenResponse
            {
                AccessToken = token,
                TokenType = "Bearer"
            };

        var uam = _rmwSettings.EcapServices.Services.FindLast(x => x.ServiceName == EcapServiceType.Uam);
        
        var uri = $"{_httpClient.BaseAddress}{uam?.ServiceVersion}{uam?.ServiceEndpoint["DeleteProfilePermanently"]}";
        
        var httpRequestMessage = _httpRequestHelper.PrepareHttpRequest(uri, HttpMethod.Post, userDeletionModel);
        
        var response = await _requestGateway.MakeRequestAsync<EcapServiceResponse>(_httpClient, httpRequestMessage, tokenResponse);
        
        return response;
    }
}