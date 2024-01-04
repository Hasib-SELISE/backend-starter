using Application.Common.Abstractions;
using Application.Common.Enums;
using Application.Common.Models;
using Application.Common.Models.ResponseModels;
using Newtonsoft.Json;

namespace Application.Common.Implementations;

public class MailServiceClient : IMailServiceClient
{
    private readonly IIdentityServiceAdapter _identityServiceAdapter;
    private readonly IHttpRequestGateway _requestGateway;
    private readonly HttpRequestHelper _httpRequestHelper;
    private readonly HttpClient _httpClient;
    private readonly IRmwSettings _rmwSettings;
    private readonly IRmwLogger<MailServiceClient> _logger;

    /// <summary>
    /// Base constructor
    /// </summary>
    /// <param name="identityServiceAdapter"></param>
    /// <param name="requestGateway"></param>
    /// <param name="httpRequestHelper"></param>
    /// <param name="httpClient"></param>
    /// <param name="rmwSettings"></param>
    /// <param name="logger"></param>
    public MailServiceClient(
        IIdentityServiceAdapter identityServiceAdapter,
        IHttpRequestGateway requestGateway,
        HttpRequestHelper httpRequestHelper,
        HttpClient httpClient,
        IRmwSettings rmwSettings,
        IRmwLogger<MailServiceClient> logger)
    {
        _identityServiceAdapter = identityServiceAdapter;
        _requestGateway = requestGateway;
        _httpRequestHelper = httpRequestHelper;
        _httpClient = httpClient;
        _rmwSettings = rmwSettings;
        _logger = logger;
    }
    
    /// <summary>
    /// SendEmailAsync
    /// </summary>
    /// <param name="user"></param>
    /// <returns></returns>
    public async Task<HttpRequestResponse<EcapServiceResponse>> SendEmailAsync(SendMailModel user)
    {
        return await SendMail("SendEmail", user);
    }

    /// <summary>
    /// SendEmailByTemplateAsync
    /// </summary>
    /// <param name="user"></param>
    /// <returns></returns>
    public async Task<HttpRequestResponse<EcapServiceResponse>> SendEmailByTemplateAsync(SendMailModel data)
    {
        return await SendMail("SendMailByTemplate", data);
    }

    /// <summary>
    /// SendEnqueueMailAsync
    /// </summary>
    /// <param name="user"></param>
    /// <returns></returns>
    public async Task<HttpRequestResponse<EcapServiceResponse>> SendEnqueueMailAsync(SendMailModel data)
    {
        return await SendMail("EnqueueMail", data);
    }
    
    /// <summary>
    /// Send Mail
    /// </summary>
    /// <param name="endPoint"></param>
    /// <param name="user"></param>
    /// <returns></returns>
    private async Task<HttpRequestResponse<EcapServiceResponse>> SendMail(string endPoint, SendMailModel user)
    {
        try
        {
            var token = await _identityServiceAdapter.GetTokenAsync(EcapCredentialType.Client);
            
            var service = _rmwSettings.EcapServices.Services.FindLast(x => x.ServiceName == EcapServiceType.Mail);
            
            var uri = $"{_httpClient.BaseAddress}{service?.ServiceVersion}{service?.ServiceEndpoint[endPoint]}";
            
            _logger.LogError(uri);
            
            var httpRequestMessage = _httpRequestHelper.PrepareHttpRequest(uri, HttpMethod.Post, user);
            
            var response = await _requestGateway.MakeRequestAsync<EcapServiceResponse>(_httpClient, httpRequestMessage, token);
            
            return response;
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, ex.Message);
            _logger.LogError($"StackTrace : {JsonConvert.SerializeObject(ex.Message)}");
            
            return new HttpRequestResponse<EcapServiceResponse>
            {
                SuccessResponse = new EcapServiceResponse
                {
                    Errors = new Error
                    {
                        IsValid = false,
                        ErrorMessage = ex.Message
                    }
                }
            };
        }
    }
}