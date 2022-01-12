using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CaptainLogger;

internal class CaptainLogger<TCategory> : ICaptainLogger<TCategory>
{
    public ILogger RuntimeLogger { get; }

    public CaptainLogger(ILogger<TCategory> logger)
    {
        RuntimeLogger = logger;
    }

    public void TraceLog(string message)
    {
        if (RuntimeLogger.IsEnabled(LogLevel.Trace))
            RuntimeLogger.LogTrace(message);
    }

    public void TraceLog(string message, Exception exception)
    {
        if (RuntimeLogger.IsEnabled(LogLevel.Trace))
            RuntimeLogger.LogTrace(exception, message);
    }

    public void DebugLog(string message)
    {
        if (RuntimeLogger.IsEnabled(LogLevel.Debug))
            RuntimeLogger.LogDebug(message);
    }

    public void DebugLog(string message, Exception exception)
    {
        if (RuntimeLogger.IsEnabled(LogLevel.Debug))
            RuntimeLogger.LogDebug(exception, message);
    }

    public void InformationLog(string message)
    {
        if (RuntimeLogger.IsEnabled(LogLevel.Information))
            RuntimeLogger.LogInformation(message);
    }

    public void InformationLog(string message, Exception exception)
    {
        if (RuntimeLogger.IsEnabled(LogLevel.Information))
            RuntimeLogger.LogInformation(exception, message);
    }

    public void WarningLog(string message)
    {
        if (RuntimeLogger.IsEnabled(LogLevel.Warning))
            RuntimeLogger.LogWarning(message);
    }

    public void WarningLog(string message, Exception exception)
    {
        if (RuntimeLogger.IsEnabled(LogLevel.Warning))
            RuntimeLogger.LogWarning(exception, message);
    }

    public void ErrorLog(string message)
    {
        if (RuntimeLogger.IsEnabled(LogLevel.Error))
            RuntimeLogger.LogError(message);
    }

    public void ErrorLog(string message, Exception exception)
    {
        if (RuntimeLogger.IsEnabled(LogLevel.Error))
            RuntimeLogger.LogError(exception, message);
    }

    public void CriticalLog(string message)
    {
        if (RuntimeLogger.IsEnabled(LogLevel.Critical))
            RuntimeLogger.LogCritical(message);
    }

    public void CriticalLog(string message, Exception exception)
    {
        if (RuntimeLogger.IsEnabled(LogLevel.Critical))
            RuntimeLogger.LogCritical(exception, message);
    }
}
