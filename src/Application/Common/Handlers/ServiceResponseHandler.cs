using System.Net;
using Application.Common.Abstractions;
using Application.Common.Models;
using FluentValidation.Results;
using Microsoft.Extensions.Logging;

namespace Application.Common.Handlers;

public static class ServiceResponseHandler
{
    public static ServiceResponse HandleSuccess(HttpStatusCode httpStatusCode)
    {
        var response = new ServiceResponse();
        response.SetSuccess(null, httpStatusCode);
        return response;
    }
    public static ServiceResponse HandleSuccess(dynamic responsedata, HttpStatusCode httpStatusCode)
    {
        var response = new ServiceResponse();
        response.SetSuccess(responsedata, httpStatusCode);
        return response;
    }
    
    public static ServiceResponse HandleError<T>(IRmwLogger<T> log, HttpStatusCode errorStatusCode, string functionName, string errorMessage, Exception? ex = null, ValidationResult validationError = null)
    {
        var exceptionDetails = ex?.ToString();
        log.LogError($"Error occurred while processing the request. Function name: {functionName}, reason: {errorMessage}, details: {exceptionDetails}");
        var response = new ServiceResponse()
        {
            Errors = validationError,
            ErrorMessage = errorMessage,
            HttpStatusCode = errorStatusCode
        };
        return response;
    }

    public static ServiceResponse HandleBadRequest(HttpStatusCode statusCode, string errorMessage)
    {        
        var response = new ServiceResponse()
        {
            Errors = null,
            ErrorMessage = errorMessage,
            HttpStatusCode = statusCode
        };
        return response;
    }
}