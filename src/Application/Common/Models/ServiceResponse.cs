using System.Net;
using FluentValidation.Results;
using Selise.Ecap.Infrastructure;

namespace Application.Common.Models;

public class ServiceResponse
{
    public dynamic Data { get; private set; }
    public bool IsSuccessful { get; set; }

    public HttpStatusCode HttpStatusCode { get; set; }
    public string? ErrorMessage { get; set; }
    public ValidationResult? Errors { get; set; }

    public void SetSuccess(dynamic data, HttpStatusCode httpStatusCode)
    {
        Data = data;
        IsSuccessful = true;
        HttpStatusCode = httpStatusCode;
    }
        
    public void SetResponseError(string propertyName, string errorMessage, HttpStatusCode httpStatusCode)
    {
        IsSuccessful = false;
        HttpStatusCode = httpStatusCode;
    }
    
    public void SetResponseError(string propertyName, string errorMessage, HttpStatusCode statusCode, ValidationResult validationResult)
    {
        Errors = validationResult;
        IsSuccessful = false;
        HttpStatusCode = statusCode;
    }
}