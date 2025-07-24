namespace CaptainLogger;

/// <summary>
/// Provides extension methods for registering CaptainLogger components into the logging pipeline.
/// </summary>
public static class Bootstrapping
{
  /// <summary>
  /// Registers CaptainLogger as a custom <see cref="ILoggerProvider"/> in the specified <paramref name="builder"/>.
  /// Also binds <see cref="CaptainLoggerOptions"/> and configures <see cref="ICaptainLogger{TCategory}"/> for DI.
  /// </summary>
  /// <param name="builder">The <see cref="ILoggingBuilder"/> to configure.</param>
  /// <returns>
  /// The same <see cref="ILoggingBuilder"/> instance, so that additional calls can be chained.
  /// </returns>
  public static ILoggingBuilder AddCaptainLogger(this ILoggingBuilder builder)
  {
    builder.AddConfiguration();

    builder
      .Services
      .AddSingleton(typeof(ICaptainLogger<>), typeof(CaptainLogger<>))
      .AddSingleton(x => (ILogDispatcher)x.GetRequiredService<ILoggerProvider>())
      .TryAddEnumerable(ServiceDescriptor.Singleton<ILoggerProvider, CaptainLoggerProvider>());

    LoggerProviderOptions
        .RegisterProviderOptions<CaptainLoggerOptions, CaptainLoggerProvider>(builder.Services);

    return builder;
  }
}
