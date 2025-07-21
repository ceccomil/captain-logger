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
    /// <param name="customCorrelationHeader"> Optional parameter to specify a custom header convention.</param>
    public static IServiceCollection AddCaptainLoggerRequestTracer(
        this IServiceCollection services,
        string? customCorrelationHeader = default)
    {
        if (services is null)
        {
            throw new ArgumentNullException(nameof(services));
        }

        if (!string.IsNullOrWhiteSpace(customCorrelationHeader))
        {
            CorrelationHeader = customCorrelationHeader;
        }

        return services
            .AddHttpContextAccessor()
            .AddSingleton<ICorrelationHandler, CorrelationHandler>()
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

        return middleware is null
            ? throw new NullReferenceException(
                $"No service {nameof(CorrelationMiddleware)} has been registered to the DI container" +
                $"{nameof(UseCaptainLoggerRequestTracer)} requires {nameof(AddCaptainLoggerRequestTracer)} " +
                "to be invoked so to register required services.")
            : builder
            .UseMiddleware<CorrelationMiddleware>();
    }
}
