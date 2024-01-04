namespace Application.Common.Models.ResponseModels;

public class FormattedMessagePlaceholderValues
{
    public string PropertyName { get; set; }
    public string PropertyValue { get; set; }
}

public class Error
{
    public string PropertyName { get; set; }
    public string ErrorMessage { get; set; }
    public string AttemptedValue { get; set; }
    public object CustomState { get; set; }
    public string Severity { get; set; }
    public string ErrorCode { get; set; }
    public List<object> FormattedMessageArguments { get; set; }
    public FormattedMessagePlaceholderValues FormattedMessagePlaceholderValues { get; set; }
    public string ResourceName { get; set; }

    public bool IsValid { get; set; }
    public List<Error> Errors { get; set; }
    public List<string> RuleSetsExecuted { get; set; }
}

public class EcapServiceResponse
{
    public Error Errors { get; set; }
    public List<string> ErrorMessages { get; set; }
    public int StatusCode { get; set; }
    public string RequestUri { get; set; }
    public object ExternalError { get; set; }
    public int HttpStatusCode { get; set; }
    public bool IsSuccessful => Errors.IsValid;
}