using Microsoft.Extensions.Logging;

namespace Application.Common.Abstractions;

public interface IRmwLogger<T>
{
        void Log(LogLevel logLevel, Exception exception, string message, params object[] args);
       
        void Log(LogLevel logLevel, EventId eventId, string message, params object[] args);
       
        void Log(LogLevel logLevel, string message, params object[] args);
        
        void Log(LogLevel logLevel, EventId eventId, Exception exception, string message, params object[] args);
       
        void LogCritical(string message, params object[] args);
        
        void LogCritical(Exception exception, string message, params object[] args);
        
        void LogCritical(EventId eventId, string message, params object[] args);
       
        void LogCritical(EventId eventId, Exception exception, string message, params object[] args);
        
        void LogDebug(EventId eventId, Exception exception, string message, params object[] args);
       
        void LogDebug(EventId eventId, string message, params object[] args);
        
        void LogDebug(Exception exception, string message, params object[] args);
       
        void LogDebug(string message, params object[] args);
        
        void LogError(string message, params object[] args);
       
        void LogError(Exception exception, string message, params object[] args);
      
        void LogError(EventId eventId, Exception exception, string message, params object[] args);
       
        void LogError(EventId eventId, string message, params object[] args);
       
        void LogInformation(EventId eventId, string message, params object[] args);
        
        void LogInformation(Exception exception, string message, params object[] args);
       
        void LogInformation(EventId eventId, Exception exception, string message, params object[] args);
       
        void LogInformation(string message, params object[] args);
        
        void LogTrace(string message, params object[] args);
        
        void LogTrace(Exception exception, string message, params object[] args);
       
        void LogTrace(EventId eventId, string message, params object[] args);
        
        void LogTrace(EventId eventId, Exception exception, string message, params object[] args);
        
        void LogWarning(EventId eventId, string message, params object[] args);
       
        void LogWarning(EventId eventId, Exception exception, string message, params object[] args);
        
        /// <summary>
        /// Log Warning 
        /// </summary>
        /// <param name="message"></param>
        /// <param name="args"></param>
        void LogWarning(string message, params object[] args);
        
        /// <summary>
        /// Log Warning With Exception
        /// </summary>
        /// <param name="exception"></param>
        /// <param name="message"></param>
        /// <param name="args"></param>
        void LogWarning(Exception exception, string message, params object[] args);
}