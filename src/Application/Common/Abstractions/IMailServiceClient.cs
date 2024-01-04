using Application.Common.Models;
using Application.Common.Models.ResponseModels;

namespace Application.Common.Abstractions;

public interface IMailServiceClient
{
    Task<HttpRequestResponse<EcapServiceResponse>> SendEmailAsync(SendMailModel data);
    Task<HttpRequestResponse<EcapServiceResponse>> SendEmailByTemplateAsync(SendMailModel data);
    Task<HttpRequestResponse<EcapServiceResponse>> SendEnqueueMailAsync(SendMailModel data);
}