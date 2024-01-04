using System.Net;

namespace Application.Common.Models.ResponseModels;

public class RestResponse
{
    public HttpStatusCode HttpStatusCode { set; get; }
    public dynamic ResponseData { get; set; }
}