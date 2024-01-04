using Application.Common.Abstractions;
using Microsoft.Extensions.Logging;

namespace Application.Common.Implementations;

public class RmwLogger<T> : IRmwLogger<T>
{
    private readonly ILogger<T> _logger;

    public RmwLogger(ILogger<T> logger)
    {
        this._logger = logger;
    }

    public void Log(LogLevel logLevel, Exception exception, string message, params object[] args)
    {
        this._logger.Log(logLevel, exception, message, args);
    }

    public void Log(LogLevel logLevel, EventId eventId, string message, params object[] args)
    {
        this._logger.Log(logLevel, eventId, message, args);
    }

    public void Log(LogLevel logLevel, string message, params object[] args)
    {
        this._logger.Log(logLevel, message, args);
    }

    public void Log(LogLevel logLevel, EventId eventId, Exception exception, string message, params object[] args)
    {
        this._logger.Log(logLevel, eventId, exception, message, args);
    }

    public void LogCritical(string message, params object[] args)
    {
        this._logger.LogCritical(message, args);
    }

    public void LogCritical(Exception exception, string message, params object[] args)
    {
        this._logger.LogCritical(exception, message, args);
    }

    public void LogCritical(EventId eventId, string message, params object[] args)
    {
        this._logger.LogCritical(eventId, message, args);
    }

    public void LogCritical(EventId eventId, Exception exception, string message, params object[] args)
    {
        this._logger.LogCritical(eventId, exception, message, args);
    }

    public void LogDebug(EventId eventId, Exception exception, string message, params object[] args)
    {
        this._logger.LogDebug(eventId, exception, message, args);
    }

    public void LogDebug(EventId eventId, string message, params object[] args)
    {
        this._logger.LogDebug(eventId, message, args);
    }

    public void LogDebug(Exception exception, string message, params object[] args)
    {
        this._logger.LogDebug(exception, message, args);
    }

    public void LogDebug(string message, params object[] args)
    {
        this._logger.LogDebug(message, args);
    }

    public void LogError(string message, params object[] args)
    {
        this._logger.LogError(message, args);
    }

    public void LogError(Exception exception, string message, params object[] args)
    {
        this._logger.LogError(exception, message, args);
    }

    public void LogError(EventId eventId, Exception exception, string message, params object[] args)
    {
        this._logger.LogError(eventId, exception, message, args);
    }

    public void LogError(EventId eventId, string message, params object[] args)
    {
        this._logger.LogError(eventId, message, args);
    }

    public void LogInformation(EventId eventId, string message, params object[] args)
    {
        this._logger.LogInformation(eventId, message, args);
    }

    public void LogInformation(Exception exception, string message, params object[] args)
    {
        this._logger.LogInformation(exception, message, args);
    }

    public void LogInformation(EventId eventId, Exception exception, string message, params object[] args)
    {
        this._logger.LogInformation(eventId, exception, message, args);
    }

    public void LogInformation(string message, params object[] args)
    {
        this._logger.LogInformation(message, args);
    }

    public void LogTrace(string message, params object[] args)
    {
        this._logger.LogTrace(message, args);
    }

    public void LogTrace(Exception exception, string message, params object[] args)
    {
        this._logger.LogTrace(exception, message, args);
    }

    public void LogTrace(EventId eventId, string message, params object[] args)
    {
        this._logger.LogTrace(eventId, message, args);
    }

    public void LogTrace(EventId eventId, Exception exception, string message, params object[] args)
    {
        this._logger.LogTrace(eventId, exception, message, args);
    }

    public void LogWarning(EventId eventId, string message, params object[] args)
    {
        this._logger.LogWarning(eventId, message, args);
    }

    public void LogWarning(EventId eventId, Exception exception, string message, params object[] args)
    {
        this._logger.LogWarning(eventId, exception, message, args);
    }

    public void LogWarning(string message, params object[] args)
    {
        this._logger.LogWarning(message, args);
    }

    public void LogWarning(Exception exception, string message, params object[] args)
    {
        this._logger.LogWarning(exception, message, args);
    }
}