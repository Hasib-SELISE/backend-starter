using System.Net.Http.Headers;
using Application.Common.Abstractions;
using Application.Common.Models.ResponseModels;
using AutoMapper;
using Microsoft.Extensions.Logging;
using Newtonsoft.Json;
using Polly.CircuitBreaker;

namespace Application.Common.Implementations;

public class HttpRequestGateway : IHttpRequestGateway
    {
        protected readonly ILogger logger;
        private readonly IMapper autoMapper;

        public HttpRequestGateway(ILogger<HttpRequestGateway> logger,
            IMapper autoMapper)
        {
            this.logger = logger;
            this.autoMapper = autoMapper;
        }

        public async Task<HttpRequestResponse<TResponse>> MakeRequestAsync<TResponse>(HttpClient client, HttpRequestMessage httpRequestMessage, IdentityTokenResponse authorizationToken = null) where TResponse : class, new()
        {
            HttpRequestResponse<TResponse> response = new HttpRequestResponse<TResponse>();
            var requestResponse = new HttpResponseMessage();

            try
            {
                logger.LogInformation(string.Format("Started processing the API request. MethodType: {0}, BaseUrl: {1}, ApiName: {2}", httpRequestMessage.Method.ToString(), client.BaseAddress, httpRequestMessage.RequestUri));

                if (authorizationToken != null)
                {
                    client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue(authorizationToken.TokenType, authorizationToken.AccessToken);
                }
                requestResponse = await client.SendAsync(httpRequestMessage);

                response = autoMapper.Map<HttpRequestResponse<TResponse>>(requestResponse);

                requestResponse.EnsureSuccessStatusCode();
                var successResponseContent = await requestResponse.Content.ReadAsStringAsync();
                var deserializedSuccessResponse = JsonConvert.DeserializeObject<TResponse>(successResponseContent);

                response.SuccessResponse = deserializedSuccessResponse;

                logger.LogInformation(string.Format("Completed processing the API request. MethodType: {0}, BaseUrl: {1}, ApiName: {2}, Response: {3}, SerializedResponse: {3}",
                                                      httpRequestMessage.Method.ToString(), client.BaseAddress, httpRequestMessage.RequestUri, successResponseContent, JsonConvert.SerializeObject(deserializedSuccessResponse)));
            }
            catch (BrokenCircuitException ex)
            {
                logger.LogError($"Circuit breaker Exception occurred while processing the API request. MethodType: {httpRequestMessage.Method}, BaseUrl: {client.BaseAddress}, ApiName: {httpRequestMessage.RequestUri}, Reason: {ex.Message}");
                throw;
            }
            catch (HttpRequestException ex)
            {
                var failedResponseContent = await requestResponse.Content.ReadAsStringAsync();
                var deserializedFailedResponse = JsonConvert.DeserializeObject<object>(failedResponseContent);
                response.FailedResponse = deserializedFailedResponse;

                logger.LogError($"Http Request Exception occurred while processing the API request. MethodType: {httpRequestMessage.Method}, BaseUrl: {client.BaseAddress}, ApiName: {httpRequestMessage.RequestUri}, Reason: {ex.Message}, ErrorResponse: {failedResponseContent}");
                throw;
            }
            return response;
        }
    }