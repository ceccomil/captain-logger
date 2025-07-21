namespace CaptainLogger;

internal class NewLoggerEvArgs : EventArgs
{
    public CptLogger Logger { get; }

    public NewLoggerEvArgs(CptLogger logger)
    {
        Logger = logger;
    }
}
