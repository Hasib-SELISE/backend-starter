namespace Application.Common.Models;

public class CommonResponseModel
{
    public bool Status { set; get; }
    public int HttpStatusCode { set; get; }
    public string ResponseMessage { set; get; }
    public dynamic ResponseData { get; set; }
}