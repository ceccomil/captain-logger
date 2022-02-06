using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Filters;
using System.Net;

namespace CaptainLogger.CentralizedLogging.Api;

public class ApiExceptionFilter : IAsyncExceptionFilter
{
    private readonly ICaptainLogger _logger;

    public ApiExceptionFilter(ICaptainLogger<ApiExceptionFilter> logger)
    {
        _logger = logger;
    }

    public Task OnExceptionAsync(ExceptionContext context)
    {
        _logger
            .ErrorLog($"Error executing {context.ActionDescriptor.DisplayName}", context.Exception);

        context
            .Result = new JsonResult(new
        {
            IsError = true,
            context.Exception.Message
        });

        context.HttpContext.Response.StatusCode = (int)HttpStatusCode.BadRequest;
        context.HttpContext.Response.ContentType = "application/json";

        return Task.CompletedTask;
    }
}
