using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CaptainLogger;

internal class CaptainLogger<TCategory> : ICaptainLogger<TCategory>
{
    public ILogger Logger { get; }

    public CaptainLogger(ILogger<TCategory> logger)
    {
       Logger = logger;
    }

    public void TraceLog(string message)
    {
        if (Logger.IsEnabled(LogLevel.Trace))
            Logger.LogTrace(message);
    }

    public void TraceLog(string message, Exception exception)
    {
        if (Logger.IsEnabled(LogLevel.Trace))
            Logger.LogTrace(exception, message);
    }

    public void DebugLog(string message)
    {
        if (Logger.IsEnabled(LogLevel.Debug))
            Logger.LogDebug(message);
    }

    public void DebugLog(string message, Exception exception)
    {
        if (Logger.IsEnabled(LogLevel.Debug))
            Logger.LogDebug(exception, message);
    }

    public void InfoLog(string message)
    {
        if (Logger.IsEnabled(LogLevel.Information))
            Logger.LogInformation(message);
    }

    public void InfoLog(string message, Exception exception)
    {
        if (Logger.IsEnabled(LogLevel.Information))
            Logger.LogInformation(exception, message);
    }

    public void WarningLog(string message)
    {
        if (Logger.IsEnabled(LogLevel.Warning))
            Logger.LogWarning(message);
    }

    public void WarningLog(string message, Exception exception)
    {
        if (Logger.IsEnabled(LogLevel.Warning))
            Logger.LogWarning(exception, message);
    }

    public void ErrorLog(string message)
    {
        if (Logger.IsEnabled(LogLevel.Error))
            Logger.LogError(message);
    }

    public void ErrorLog(string message, Exception exception)
    {
        if (Logger.IsEnabled(LogLevel.Error))
            Logger.LogError(exception, message);
    }

    public void CriticalLog(string message)
    {
        if (Logger.IsEnabled(LogLevel.Critical))
            Logger.LogCritical(message);
    }

    public void CriticalLog(string message, Exception exception)
    {
        if (Logger.IsEnabled(LogLevel.Critical))
            Logger.LogCritical(exception, message);
    }
}
