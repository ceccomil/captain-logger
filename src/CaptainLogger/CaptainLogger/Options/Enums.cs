namespace CaptainLogger.Options;

/// <summary>
/// 
/// </summary>
[Flags]
public enum Recipients
{
    /// <summary>
    /// 
    /// </summary>
    None = 0,

    /// <summary>
    /// 
    /// </summary>
    Console = 1,

    /// <summary>
    /// 
    /// </summary>
    File = 2
}

/// <summary>
/// 
/// </summary>
public enum LogRotation
{
    /// <summary>
    /// 
    /// </summary>
    None = 0,

    /// <summary>
    /// 
    /// </summary>
    Year = 1,

    /// <summary>
    /// 
    /// </summary>
    Month = 2,

    /// <summary>
    /// 
    /// </summary>
    Day = 3,

    /// <summary>
    /// 
    /// </summary>
    Hour = 4,

    /// <summary>
    /// 
    /// </summary>
    Minute = 5
}
