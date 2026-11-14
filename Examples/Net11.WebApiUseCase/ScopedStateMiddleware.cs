namespace WebApiUseCase;

internal sealed class ScopedStateMiddleware(
  ILogger<ScopedStateMiddleware> logger) : IMiddleware
{
  private readonly ILogger _logger = logger;

  public async Task InvokeAsync(
    HttpContext context,
    RequestDelegate next)
  {
    var correlationId = Guid.NewGuid();

    var stateDict = new Dictionary<string, object>()
    {
      ["MyScopedGuid"] = Guid.NewGuid(),
      ["MyScopedCorrelationId"] = correlationId
    };

    using var propertiesScopes = _logger.BeginScope(stateDict);
    using var correlationScope = CaptainLoggerCorrelationScope.BeginScope(correlationId);

    await next(context);
  }
}
