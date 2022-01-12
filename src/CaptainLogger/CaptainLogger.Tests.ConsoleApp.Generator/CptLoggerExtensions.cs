
//namespace CaptainLogger;

///// <summary>
///// CptLoggerExtensions
///// </summary>
//public static class CptLoggerExtensions
//{
//    /// <summary>
//    /// Extends TraceLog with arguments
//    /// </summary>
//    public static void TraceLog<T0>(
//        this ICaptainLogger cpt,
//        string message,
//        T0 arg0)
//    {
//        if (cpt.Logger.IsEnabled(LogLevel.Trace))
//            cpt.Logger.LogTrace(
//                message,
//                arg0);
//    }

//    /// <summary>
//    /// Extends TraceLog with arguments
//    /// </summary>
//    public static void TraceLog<T0>(
//        this ICaptainLogger cpt,
//        string message,
//        T0 arg0,
//        Exception exception)
//    {
//        if (cpt.Logger.IsEnabled(LogLevel.Trace))
//            cpt.Logger.LogTrace(
//                exception,
//                message,
//                arg0);
//    }
//}
