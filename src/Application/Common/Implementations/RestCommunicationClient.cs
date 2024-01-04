using System.Net;
using System.Net.Http.Headers;
using System.Text;
using Application.Common.Abstractions;
using Application.Common.Enums;
using Application.Common.Models;
using Application.Common.Models.ResponseModels;
using Microsoft.Extensions.Configuration;
using Newtonsoft.Json;
using Selise.Ecap.Infrastructure;

namespace Application.Common.Implementations;

public class RestCommunicationClient : IRestCommunicationClient
{
    private readonly IIdentityServiceAdapter _identityServiceAdapter;
    private readonly IServiceClient _serviceClient;
    private readonly IConfiguration _configuration;
    private readonly IRmwSecurityContextProvider _securityContextProvider;
    private readonly IAppSettings _appSettings;
    private readonly IRmwLogger<RestCommunicationClient> _logger;

    public RestCommunicationClient(
        IIdentityServiceAdapter identityServiceAdapter,
        IServiceClient serviceClient, 
        IConfiguration configuration, 
        IRmwSecurityContextProvider securityContextProvider, 
        IAppSettings appSettings, 
        IRmwLogger<RestCommunicationClient> logger)
    {
        _identityServiceAdapter = identityServiceAdapter;
        this._serviceClient = serviceClient;
        this._configuration = configuration;
        this._securityContextProvider = securityContextProvider;
        this._appSettings = appSettings;
        this._logger = logger;
    }

    public async Task<string> PostRequestAsync<TParam>(Uri uri, TParam payload)
    {
        var responseJson = "";

        using (var request = new HttpRequestMessage(HttpMethod.Post, uri))
        {
            var requestBody = JsonConvert.SerializeObject(payload);


            using (request.Content = new StringContent(requestBody, Encoding.UTF8, "application/json"))
            {
                request.Headers.Authorization = new AuthenticationHeaderValue("bearer",
                    _securityContextProvider.GetSecurityContext().OauthBearerToken);

                try
                {
                    var response = await _serviceClient.SendToHttpAsync(request);

                    if (response.IsSuccessStatusCode)
                    {
                        responseJson = await response.Content.ReadAsStringAsync();
                    }
                    else
                    {
                    }
                }
                catch (Exception e)
                {
                    _logger.LogError(e.Message);
                }
            }
        }

        return responseJson;
    }

    public async Task<string> PostRequestAsync<TParam>(Uri uri, TParam payload, string token, int requestCounter = 0)
    {
        var requestBody = JsonConvert.SerializeObject(payload);
        var responseJson = "";

        //_logger.LogError($"{Environment.NewLine}uri: {uri.ToString()}{Environment.NewLine}");
        //_logger.LogError($"{Environment.NewLine}payload: {requestBody} {Environment.NewLine}");

        using (var request = new HttpRequestMessage(HttpMethod.Post, uri))
        {
            using (request.Content = new StringContent(requestBody, Encoding.UTF8, "application/json"))
            {
                request.Headers.Authorization = new AuthenticationHeaderValue("bearer", token);
                try
                {
                    var response = await _serviceClient.SendToHttpAsync(request);
                    switch (response.StatusCode)
                    {
                        case System.Net.HttpStatusCode.Forbidden:
                        case System.Net.HttpStatusCode.Unauthorized:
                            if (requestCounter < 2)
                            {
                                var tokenStr = await GetTokenAsync(TokenType.Admin);
                                var tokendata = JsonConvert.DeserializeObject<TokenModel>(tokenStr);
                                //request.Headers.Authorization = new AuthenticationHeaderValue("bearer", tokendata.access_token);
                                requestCounter++;
                                responseJson = await PostRequestAsync(uri, payload, tokendata.access_token,
                                    requestCounter);
                            }

                            break;
                        case System.Net.HttpStatusCode.OK:
                            if (response.IsSuccessStatusCode)
                            {
                                responseJson = await response.Content.ReadAsStringAsync();
                            }

                            break;
                    }
                }
                catch (Exception e)
                {
                    _logger.LogError("error: " + e.Message);
                }
            }
        }

        return responseJson;
    }

    public async Task<string> PostRequestAsync<TParam>(Uri uri, TParam payload, TokenType tokenType,
        int requestCounter = 0, int requested = 0)
    {
        var requestBody = JsonConvert.SerializeObject(payload);
        var responseJson = "";

        var tokenAdmin = await _identityServiceAdapter.GetTokenAsync(EcapCredentialType.Client);

        // await Task.Delay(1000);
        
        // var tokenStr = await GetTokenAsync(tokenType);
        /*var token = JsonConvert.DeserializeObject<TokenModel>(tokenStr);*/
        var token = tokenAdmin.AccessToken;

        using var request = new HttpRequestMessage(HttpMethod.Post, uri);
        using (request.Content = new StringContent(requestBody, Encoding.UTF8, "application/json"))
        {
            request.Headers.Authorization = new AuthenticationHeaderValue("bearer", token);
            try
            {
                var response = await _serviceClient.SendToHttpAsync(request);
                requested++;

                if (response.StatusCode != System.Net.HttpStatusCode.OK && requested < requestCounter)
                {
                    requested++;
                    responseJson = await PostRequestAsync(uri, payload, tokenType, requestCounter, requested);
                }
                else
                {
                    responseJson = await response.Content.ReadAsStringAsync();
                }
            }
            catch (Exception exception)
            {
                _logger.LogError(exception.Message);
            }
        }

        return responseJson;
    }

    public async Task<string> GetRequestAsync(Uri uri)
    {
        var responseJson = "";

        using (var request = new HttpRequestMessage(HttpMethod.Get, uri))
        {
            try
            {
                var response = await _serviceClient.SendToHttpAsync(request);

                if (response.IsSuccessStatusCode)
                {
                    responseJson = await response.Content.ReadAsStringAsync();
                }
            }
            catch (Exception e)
            {
                _logger.LogError(e.Message);
            }
        }

        return responseJson;
    }

    public async Task<HttpStatusCode> PutRequestAsync(MemoryStream stream, Uri uri,
        Dictionary<string, string> headers = null)
    {
        var restResponse = new HttpResponseMessage();
        var binaryStream = stream.ToArray();

        using (var request = new HttpRequestMessage(HttpMethod.Put, uri))
        {
            using (request.Content = new ByteArrayContent(binaryStream))
            {
                request.Content.Headers.ContentType = new MediaTypeHeaderValue("application/octet-stream");
                if (headers != null)
                {
                    foreach (var key in headers.Keys)
                    {
                        request.Content.Headers.Add(key, headers[key]);
                    }
                }

                try
                {
                    restResponse = await _serviceClient.SendToHttpAsync(request);
                }
                catch (Exception ex)
                {
                    _logger.LogError($"Error occurred while storing file- {ex.ToString()}");
                }
            }
        }

        return restResponse.StatusCode;
    }

    public async Task<RestResponse> GetRequestAsync(Uri uri, TokenType tokenType, int requestCounter = 0,
        int requested = 0)
    {
        var commonResponse = new RestResponse();

        var tokenAdmin = await _identityServiceAdapter.GetTokenAsync(EcapCredentialType.Client);
        var token = tokenAdmin.AccessToken;

        using (var request = new HttpRequestMessage(HttpMethod.Get, uri))
        {
            request.Headers.Authorization = new AuthenticationHeaderValue("bearer", token);

            try
            {
                var response = await _serviceClient.SendToHttpAsync(request);

                commonResponse.HttpStatusCode = response.StatusCode;
                commonResponse.ResponseData = await response.Content.ReadAsStringAsync();

                requested++;

                if (response.StatusCode != System.Net.HttpStatusCode.OK && requested < requestCounter)
                {
                    commonResponse = await GetRequestAsync(uri, tokenType, requestCounter, requested);
                }
            }
            catch (Exception ex)
            {
                _logger.LogError($"Error occurred while Getting file- {ex.ToString()}");
            }
        }

        return commonResponse;
    }

    public async Task<Stream> DownloadFileStream(Uri uri)
    {
        var responseStream = Stream.Null;

        using (var request = new HttpRequestMessage(HttpMethod.Get, uri))
        {
            try
            {
                var response = await _serviceClient.SendToHttpAsync(request);

                if (response.IsSuccessStatusCode)
                {
                    responseStream = await response.Content.ReadAsStreamAsync();
                }
            }
            catch (Exception e)
            {
                _logger.LogError(e.Message);
            }
        }

        return responseStream;
    }

    public async Task<string> GetTokenAsync(TokenType tokentype = TokenType.Anonymous)
    {
        var tokenBaseUrl = _configuration.GetSection("TokenBaseUrl").Value;
        if (string.IsNullOrWhiteSpace(tokenBaseUrl))
        {
            tokenBaseUrl = _appSettings.IdentityServiceBaseUri;
        }

        var tokenUri = $"{tokenBaseUrl}{_configuration.GetSection("TokenEndpoint").Value}";

        var origin = _configuration.GetSection("ApplicationOrigin").Value;

        var responseJson = "";

        var payload = new List<KeyValuePair<string, string>>();
        payload.Add(new KeyValuePair<string, string>("grant_type",
            _configuration.GetSection("AnonymousGrantType").Value));

        if (tokentype == TokenType.Admin)
        {
            payload.Clear();
            payload.Add(new KeyValuePair<string, string>("grant_type",
                _configuration.GetSection("AdminGrantType").Value));
            payload.Add(new KeyValuePair<string, string>("client_id",
                _configuration.GetSection("AdminClientId").Value));
            payload.Add(new KeyValuePair<string, string>("client_secret",
                _configuration.GetSection("AdminClientSecrete").Value));
        }

        using (var request = new HttpRequestMessage(HttpMethod.Post, tokenUri))
        {
            using (request.Content = new FormUrlEncodedContent(payload))
            {
                request.Headers.Add("Origin", origin);
                var response = await _serviceClient.SendToHttpAsync(request);
                if (response.IsSuccessStatusCode)
                {
                    responseJson = await response.Content.ReadAsStringAsync();
                }
            }
        }

        return responseJson;
    }
}