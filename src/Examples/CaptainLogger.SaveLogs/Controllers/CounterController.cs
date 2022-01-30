using CaptainLogger.SaveLogs.Logging;
using Microsoft.AspNetCore.Mvc;

namespace CaptainLogger.SaveLogs.Controllers;

[ApiController]
[Route("[controller]")]
public class CounterController : ControllerBase
{
    private static int _counter = 0;

    private readonly ICaptainLogger _logger;

    public CounterController(
        ICaptainLogger<CounterController> logger,
        ILogHandler logHandler)
    {
        _logger = logger;
        logHandler.SubscribeToLoggerEvents();
    }

    [HttpGet]
    public IActionResult GetCounter()
    {
        _counter++;

        _logger.InformationLog(_counter);

        return Ok(_counter);
    }
}
