namespace CaptainLogger.RequestTracer;

/// <summary>
/// <see cref="TracerExtensions"/>
/// </summary>
public static class TracerExtensions
{
    /// <summary>
    /// Adds the tracer service to the specified <see cref="IServiceCollection" />.
    /// </summary>
    /// <param name="services">The <see cref="IServiceCollection" /> to add the middleware to.</param>
    public static IServiceCollection AddCaptainLoggerRequestTracer(
        this IServiceCollection services)
    {
        if (services is null)
        {
            throw new ArgumentNullException(nameof(services));
        }

        return services
            .AddHttpContextAccessor()
            .AddSingleton<ICorrelationHeader, CorrelationHeader>()
            .AddTransient<CorrelationMiddleware>();
    }

    /// <summary>
    /// Adds the tracer middleware to the <see cref="IApplicationBuilder"/> request execution pipeline.
    /// </summary>
    /// <param name="builder">The <see cref="IApplicationBuilder"/>.</param>
    public static IApplicationBuilder UseCaptainLoggerRequestTracer(
        this IApplicationBuilder builder)
    {
        if (builder is null)
        {
            throw new ArgumentNullException(nameof(builder));
        }

        var middleware = builder
            .ApplicationServices
            .GetService<CorrelationMiddleware>();

        if (middleware is null)
        {
            throw new NullReferenceException(
                $"No service {nameof(CorrelationMiddleware)} has been registered to the DI container" +
                $"{nameof(UseCaptainLoggerRequestTracer)} requires {nameof(AddCaptainLoggerRequestTracer)} " +
                "to be invoked so to register required services.");
        }

        return builder
            .UseMiddleware<CorrelationMiddleware>();
    }
}
