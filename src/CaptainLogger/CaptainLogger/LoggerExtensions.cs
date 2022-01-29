using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.DependencyInjection.Extensions;
using Microsoft.Extensions.Logging.Configuration;

namespace CaptainLogger;


/// <summary>
/// <see cref="LoggerExtensions"/>
/// </summary>
public static class LoggerExtensions
{
    internal static string ToCaptainLoggerString(this LogLevel logLevel) => logLevel switch
    {
        LogLevel.Error => "ERR",
        LogLevel.Warning => "WRN",
        LogLevel.Critical => "CRT",
        LogLevel.Debug => "DBG",
        LogLevel.Trace => "TRC",
        _ => "INF",
    };

    /// <summary>
    /// Adds <see cref="ILoggerProvider"/> and registers <c>CaptainLogger</c>
    /// custom provider to the <paramref name="builder"/>.
    /// </summary>
    /// <param name="builder">The <see cref="ILoggingBuilder"/>.</param>
    /// <returns>A reference to the current instance of <see cref="ILoggingBuilder"/>
    /// so that additional calls can be chained..</returns>
    public static ILoggingBuilder AddCaptainLogger(this ILoggingBuilder builder)
    {
        builder
            .AddConfiguration();

        builder
            .Services
            .AddSingleton(typeof(ICaptainLogger<>), typeof(CaptainLoggerBase<>))
            .TryAddEnumerable(ServiceDescriptor.Singleton<ILoggerProvider, LoggerProvider>());

        LoggerProviderOptions
            .RegisterProviderOptions<CaptainLoggerOptions, LoggerProvider>(builder.Services);

        return builder;
    }
}
