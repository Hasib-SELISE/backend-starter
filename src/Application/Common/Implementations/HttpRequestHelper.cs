using System.Text;
using Application.Common.Settings;
using Newtonsoft.Json;

namespace Application.Common.Implementations;

public class HttpRequestHelper
{

    public virtual HttpRequestMessage PrepareHttpRequest(string requestUrl,
        HttpMethod httpRequestType,
        object content = null,
        StreamContent streamContent = null)
    {
        var httpRequestMessage = new HttpRequestMessage
        {
            Method = httpRequestType,
            RequestUri = new Uri(requestUrl)
        };
        if (content != null)
        {
            var jsonContent = new StringContent(JsonConvert.SerializeObject(content), Encoding.UTF8, "application/json");

            httpRequestMessage.Content = jsonContent;
        }
        if (streamContent != null)
        {
            httpRequestMessage.Content = streamContent;
        }
        return httpRequestMessage;
    }

    public virtual HttpRequestMessage PrepareHttpRequest(string requestUrl,
        HttpMethod httpRequestType,
        Credential credential)
    {
        var contentPayloads = new List<KeyValuePair<string, string>>();

        foreach (var item in credential.CredentialPayloads)
            contentPayloads.Add(new KeyValuePair<string, string>(item.Key, item.Value));

        var request = new HttpRequestMessage
        {
            Method = httpRequestType,
            RequestUri = new Uri(requestUrl),
            Content = new FormUrlEncodedContent(contentPayloads)
        };
        request.Content.Headers.Clear();
        request.Content.Headers.Add("Content-Type", "application/x-www-form-urlencoded");
        request.Content.Headers.Add("Origin", credential.OriginUrl);

        return request;
    }
}