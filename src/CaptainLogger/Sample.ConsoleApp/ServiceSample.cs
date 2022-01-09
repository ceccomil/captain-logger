using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using System.Text.Json;

namespace Sample.ConsoleApp;

public class ServiceSample : IServiceSample
{
    private readonly ILogger _logger;
    private readonly LoggerFilterOptions _lOpts;

    public ServiceSample(
        ILogger<ServiceSample> logger,
        IOptions<LoggerFilterOptions> lOpts)
    {
        _logger = logger;
        _lOpts = lOpts.Value;
    }

    public async Task RunAsync()
    {
        await WriteSomeLogs();
    }

    private async Task WriteSomeLogs()
    {
        var delay = 3000;

        await LogTraceSample(delay);

        _logger
            .LogDebug("Debug log: next await 1sec");

        await Task.Delay(delay);

        _logger
            .LogInformation("Information log: next await 1sec");

        await Task.Delay(delay);

        _logger
            .LogWarning("Warning log: next await 1sec");

        await Task.Delay(delay);

        _logger
            .LogError("Error log: next await 1sec");

        await Task.Delay(delay);

        _logger
            .LogCritical("Critical log: next await 1sec");

        await Task.Delay(delay);

        try
        {
            throw new ApplicationException("ApplicationException Example Message!");
        }
        catch (Exception ex1)
        {
            try
            {
                throw new NotImplementedException("NotImplementedException!", ex1);
            }
            catch (Exception ex2)
            {
                _logger.LogError(ex2, "Error with an exception!");
            }
        }
    }

    private async Task LogTraceSample(int delay)
    {
        _logger
            .LogTrace("Trace log:\r\n{Json}",
            JsonSerializer.Serialize(
            new
            {
                TestValue1 = "TestValue1",
                TestValue2 = 2,
                TestValue3 = DateTime.Now
            },
            new JsonSerializerOptions(JsonSerializerDefaults.Web)
            {
                WriteIndented = true
            }));

        await Task.Delay(delay);
    }
}