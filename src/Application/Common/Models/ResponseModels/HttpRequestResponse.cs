namespace Application.Common.Models.ResponseModels;

public class HttpRequestResponse<TSuccessResponse> where TSuccessResponse : class, new()
{
    public TSuccessResponse SuccessResponse { get; set; }
    public object FailedResponse { get; set; }
}