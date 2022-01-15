﻿using Microsoft.Extensions.Logging;

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
    /// <para>For High-performance logging use CaptainLogger.Extensions.Generator</para>
    /// </summary>
    void TraceLog(string message);

    /// <summary>
    /// If enabled, write a <see cref="LogLevel.Trace"/> log entry
    /// <para>For High-performance logging use CaptainLogger.Extensions.Generator</para>
    /// </summary>
    void TraceLog(string message, Exception exception);

    /// <summary>
    /// If enabled, write a <see cref="LogLevel.Debug"/> log entry
    /// <para>For High-performance logging use CaptainLogger.Extensions.Generator</para>
    /// </summary>
    void DebugLog(string message);

    /// <summary>
    /// If enabled, write a <see cref="LogLevel.Debug"/> log entry
    /// <para>For High-performance logging use CaptainLogger.Extensions.Generator</para>
    /// </summary>
    void DebugLog(string message, Exception exception);

    /// <summary>
    /// If enabled, write a <see cref="LogLevel.Information"/> log entry
    /// <para>For High-performance logging use CaptainLogger.Extensions.Generator</para>
    /// </summary>
    void InformationLog(string message);

    /// <summary>
    /// If enabled, write a <see cref="LogLevel.Information"/> log entry
    /// <para>For High-performance logging use CaptainLogger.Extensions.Generator</para>
    /// </summary>
    void InformationLog(string message, Exception exception);

    /// <summary>
    /// If enabled, write a <see cref="LogLevel.Warning"/> log entry
    /// <para>For High-performance logging use CaptainLogger.Extensions.Generator</para>
    /// </summary>
    void WarningLog(string message);

    /// <summary>
    /// If enabled, write a <see cref="LogLevel.Warning"/> log entry
    /// <para>For High-performance logging use CaptainLogger.Extensions.Generator</para>
    /// </summary>
    void WarningLog(string message, Exception exception);

    /// <summary>
    /// If enabled, write a <see cref="LogLevel.Error"/> log entry
    /// <para>For High-performance logging use CaptainLogger.Extensions.Generator</para>
    /// </summary>
    void ErrorLog(string message);

    /// <summary>
    /// If enabled, write a <see cref="LogLevel.Error"/> log entry
    /// <para>For High-performance logging use CaptainLogger.Extensions.Generator</para>
    /// </summary>
    void ErrorLog(string message, Exception exception);

    /// <summary>
    /// If enabled, write a <see cref="LogLevel.Critical"/> log entry
    /// <para>For High-performance logging use CaptainLogger.Extensions.Generator</para>
    /// </summary>
    void CriticalLog(string message);

    /// <summary>
    /// If enabled, write a <see cref="LogLevel.Critical"/> log entry
    /// <para>For High-performance logging use CaptainLogger.Extensions.Generator</para>
    /// </summary>
    void CriticalLog(string message, Exception exception);
}

/// <summary>
/// CaptainLogger where logger category name is derived by the <see cref="TCategory"/> Type
/// </summary>
/// <typeparam name="TCategory"></typeparam>
public interface ICaptainLogger<out TCategory> : ICaptainLogger { }