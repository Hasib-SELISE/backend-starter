using Application.Common.Abstractions;
using Application.Common.Enums;
using Application.Common.Models;
using Microsoft.Extensions.Configuration;
using Newtonsoft.Json;
using System.Net;

namespace Application.Common.Implementations
{
    public class SequenceNumberServiceAdapter : ISequenceNumberServiceAdapter
    {

        private readonly IRestCommunicationClient _restCommunicationClient;
        private readonly IConfiguration _configuration;

        public SequenceNumberServiceAdapter( IRestCommunicationClient restCommunicationClient,
            IConfiguration configuration)
        {
            _restCommunicationClient = restCommunicationClient;
            _configuration = configuration;
        }


        public async Task<SequenceNumberQueryResponse> GetSequenceNumberAsync(string context)
        {
            var apiUrl = _configuration["SequenceNumberBaseUrl"];
            var apiEndpoint = _configuration["SequenceNumberEndpoint"];
            var uri = new Uri($"{apiUrl}{apiEndpoint}?Context={context}");
            var response = await _restCommunicationClient.GetRequestAsync(uri, TokenType.Anonymous);
            var numberModel = new SequenceNumberQueryResponse();
            if (response.HttpStatusCode == HttpStatusCode.OK)
                numberModel = JsonConvert.DeserializeObject<SequenceNumberQueryResponse>(response.ResponseData);
            return numberModel;
        }
    }
}
