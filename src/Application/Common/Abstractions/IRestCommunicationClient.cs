using System.Net;
using Application.Common.Enums;
using Application.Common.Models;
using Application.Common.Models.ResponseModels;

namespace Application.Common.Abstractions;

public interface IRestCommunicationClient
{
    Task<string> PostRequestAsync<TParam>(Uri uri, TParam payload);
    Task<string> PostRequestAsync<TParam>(Uri uri, TParam payload, string token, int requestCounter = 0);
    Task<string> PostRequestAsync<TParam>(Uri uri, TParam payload, TokenType tokenType, int requestCounter = 0, int requested = 0);
    Task<string> GetRequestAsync(Uri uri);
    Task<string> GetTokenAsync(TokenType tokentype = TokenType.Anonymous);

    Task<Stream> DownloadFileStream(Uri uri);
    Task<RestResponse> GetRequestAsync(Uri uri, TokenType tokenType, int requestCounter = 0, int requested = 0);
    Task<HttpStatusCode> PutRequestAsync(MemoryStream stream, Uri uri, Dictionary<string, string> headers = null);
}