using Microsoft.Extensions.Logging;

namespace CaptainLogger;

/// <summary>
/// CaptainLogger
/// </summary>
public interface ICaptainLogger
{
    /// <summary>
    /// The underline <see cref="ILogger"/> used to perform logging
    /// </summary>
    ILogger RuntimeLogger { get; }

    /// <summary>
    /// If enabled, write a <see cref="LogLevel.Trace"/> log entry
    /// </summary>
    void TraceLog(string message);

    /// <summary>
    /// If enabled, write a <see cref="LogLevel.Trace"/> log entry
    /// </summary>
    void TraceLog(string message, Exception exception);

    /// <summary>
    /// If enabled, write a <see cref="LogLevel.Debug"/> log entry
    /// </summary>
    void DebugLog(string message);

    /// <summary>
    /// If enabled, write a <see cref="LogLevel.Debug"/> log entry
    /// </summary>
    void DebugLog(string message, Exception exception);

    /// <summary>
    /// If enabled, write a <see cref="LogLevel.Information"/> log entry
    /// </summary>
    void InformationLog(string message);

    /// <summary>
    /// If enabled, write a <see cref="LogLevel.Information"/> log entry
    /// </summary>
    void InformationLog(string message, Exception exception);

    /// <summary>
    /// If enabled, write a <see cref="LogLevel.Warning"/> log entry
    /// </summary>
    void WarningLog(string message);

    /// <summary>
    /// If enabled, write a <see cref="LogLevel.Warning"/> log entry
    /// </summary>
    void WarningLog(string message, Exception exception);

    /// <summary>
    /// If enabled, write a <see cref="LogLevel.Error"/> log entry
    /// </summary>
    void ErrorLog(string message);

    /// <summary>
    /// If enabled, write a <see cref="LogLevel.Error"/> log entry
    /// </summary>
    void ErrorLog(string message, Exception exception);

    /// <summary>
    /// If enabled, write a <see cref="LogLevel.Critical"/> log entry
    /// </summary>
    void CriticalLog(string message);

    /// <summary>
    /// If enabled, write a <see cref="LogLevel.Critical"/> log entry
    /// </summary>
    void CriticalLog(string message, Exception exception);
}

/// <summary>
/// CaptainLogger where logger category name is derived by the <see cref="TCategory"/> Type
/// </summary>
/// <typeparam name="TCategory"></typeparam>
public interface ICaptainLogger<out TCategory> : ICaptainLogger { }