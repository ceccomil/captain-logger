namespace CaptainLogger.Options;

/// <summary>
/// Medium recipients for CaptainLogger
/// </summary>
[Flags]
public enum Recipients
{
    /// <summary>
    /// No logs written
    /// </summary>
    None = 0,

    /// <summary>
    /// Write logs to Console
    /// </summary>
    Console = 1,

    /// <summary>
    /// Writes logs to File
    /// </summary>
    File = 2,

    /// <summary>
    /// Writes logs to a stream
    /// </summary>
    Stream = 4
}

/// <summary>
/// If the current configuration (<see cref="CaptainLoggerOptions"/>) has flag <see cref="Recipients.File"/>,
/// intervals will be applied to the log fileName
/// </summary>
public enum LogRotation
{
    /// <summary>
    /// Infinite time interval
    /// </summary>
    None = 0,

    /// <summary>
    /// FileName change every Year
    /// </summary>
    Year = 1,

    /// <summary>
    /// FileName change every Month
    /// </summary>
    Month = 2,

    /// <summary>
    /// FileName change every Day
    /// </summary>
    Day = 3,

    /// <summary>
    /// FileName change every Hour
    /// </summary>
    Hour = 4,

    /// <summary>
    /// FileName change every Minute
    /// </summary>
    Minute = 5
}

/// <summary>
/// Used by CaptainLogger.Extensions.Generator to establish how many
/// (up to seven) overloads of "LevelLog()" exthension methods will be created
/// </summary>
public enum LogArguments
{
    /// <summary>
    /// Zero = 0
    /// </summary>
    Zero = 0,

    /// <summary>
    /// One = 1
    /// </summary>
    One = 1,

    /// <summary>
    /// Two = 2
    /// </summary>
    Two = 2,

    /// <summary>
    /// Three = 3
    /// </summary>
    Three = 3,

    /// <summary>
    /// Four = 4
    /// </summary>
    Four = 4,

    /// <summary>
    /// Five = 5
    /// </summary>
    Five = 5,

    /// <summary>
    /// Six = 6
    /// </summary>
    Six = 6,

    /// <summary>
    /// Seven = 7
    /// </summary>
    Seven = 7
}