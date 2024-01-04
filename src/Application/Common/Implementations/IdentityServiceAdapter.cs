using Application.Common.Abstractions;
using Application.Common.Enums;
using Application.Common.Models.ResponseModels;
using Microsoft.Extensions.Logging;

namespace Application.Common.Implementations;

public class IdentityServiceAdapter : IIdentityServiceAdapter
    {
        private readonly IHttpRequestGateway httpRequestGateway;
        private readonly HttpClient client;
        private readonly ILogger<IdentityServiceAdapter> logger;
        private readonly IRmwSettings rmwSettings;
        private readonly HttpRequestHelper prepareHttpRequestHelper;

        public IdentityServiceAdapter(IHttpRequestGateway httpRequestGateway,
            HttpClient client,
            ILogger<IdentityServiceAdapter> logger,
            IRmwSettings rmwSettings,
            HttpRequestHelper prepareHttpRequestHelper)
        {
            this.httpRequestGateway = httpRequestGateway;
            this.client = client;
            this.logger = logger;
            this.rmwSettings = rmwSettings;
            this.prepareHttpRequestHelper = prepareHttpRequestHelper;
        }
        public async Task<IdentityTokenResponse> GetTokenAsync(EcapCredentialType credentialType)
        {
            logger.LogInformation("Creating Token...");
            var response = new IdentityTokenResponse();
            try
            {
                var api = rmwSettings.EcapServices.Services.FindLast(x => x.ServiceName == EcapServiceType.Identity);
                var credential = rmwSettings.EcapCredentials.Credentials.FindLast(x => x.CredentialType == credentialType);
                string uri = $"{client.BaseAddress}{api.ServiceVersion}{api.ServiceEndpoint["GetToken"]}";
                
                logger.LogInformation($"Identity URL: {uri}");
                
                var httpRequestMessage = prepareHttpRequestHelper.PrepareHttpRequest(uri, HttpMethod.Post, credential);
                var result = await httpRequestGateway.MakeRequestAsync<IdentityTokenResponse>(client, httpRequestMessage, null);

                response = result.SuccessResponse;
            }
            catch (Exception exception)
            {
                logger.LogError($"Unhandled exception in GetTokenAsync for credentialType: {credentialType}", exception);
            }
            return response;
        }
    }