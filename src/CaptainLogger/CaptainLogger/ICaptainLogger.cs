namespace CaptainLogger;

/// <summary>
/// 
/// </summary>
public interface ICaptainLogger
{
    /// <summary>
    /// 
    /// </summary>
    ILogger Logger { get; }

    /// <summary>
    /// TraceLog
    /// </summary>
    /// <param name="message"></param>
    void TraceLog(string message);

    /// <summary>
    /// 
    /// </summary>
    /// <param name="message"></param>
    /// <param name="exception"></param>
    void TraceLog(string message, Exception exception);

    /// <summary>
    /// 
    /// </summary>
    /// <param name="message"></param>
    void DebugLog(string message);

    /// <summary>
    /// 
    /// </summary>
    /// <param name="message"></param>
    /// <param name="exception"></param>
    void DebugLog(string message, Exception exception);

    /// <summary>
    /// 
    /// </summary>
    /// <param name="message"></param>
    void InfoLog(string message);

    /// <summary>
    /// 
    /// </summary>
    /// <param name="message"></param>
    /// <param name="exception"></param>
    void InfoLog(string message, Exception exception);

    /// <summary>
    /// 
    /// </summary>
    /// <param name="message"></param>
    void WarningLog(string message);

    /// <summary>
    /// 
    /// </summary>
    /// <param name="message"></param>
    /// <param name="exception"></param>
    void WarningLog(string message, Exception exception);

    /// <summary>
    /// 
    /// </summary>
    /// <param name="message"></param>
    void ErrorLog(string message);

    /// <summary>
    /// 
    /// </summary>
    /// <param name="message"></param>
    /// <param name="exception"></param>
    void ErrorLog(string message, Exception exception);

    /// <summary>
    /// 
    /// </summary>
    /// <param name="message"></param>
    void CriticalLog(string message);

    /// <summary>
    /// 
    /// </summary>
    /// <param name="message"></param>
    /// <param name="exception"></param>
    void CriticalLog(string message, Exception exception);
}

/// <summary>
/// 
/// </summary>
/// <typeparam name="TCategory"></typeparam>
public interface ICaptainLogger<out TCategory> : ICaptainLogger { }