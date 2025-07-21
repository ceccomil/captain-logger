using CaptainLogger.SaveLogs.Logging;
using Microsoft.AspNetCore.Mvc;

namespace CaptainLogger.SaveLogs.Controllers;

[ApiController]
[Route("[controller]")]
public class LogsController : ControllerBase
{
    private readonly IRepo _repo;

    public LogsController(IRepo repo)
    {
        _repo = repo;
    }

    [HttpGet(Name = "GetLogs")]
    public async Task<IEnumerable<LogEntry>> Get() => await _repo.GetAll();
}