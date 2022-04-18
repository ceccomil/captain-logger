﻿using System.Linq;

namespace CaptainLogger.RequestTracer;

internal class CorrelationMiddleware : IMiddleware
{
    public async Task InvokeAsync(
        HttpContext context,
        RequestDelegate next)
    {
        var headerValues = context
            .Request
            .Headers[CORRELATION_HEADER];

        if (!StringValues.IsNullOrEmpty(headerValues))
        {
            var headerValue = string.Join('-', headerValues);
            context.TraceIdentifier = headerValue;
        }

        await next(context);
    }
}