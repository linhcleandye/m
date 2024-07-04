using System.Diagnostics;
using Microsoft.AspNetCore.Http;
using Serilog;

namespace Infrastructure.Middlewares;

public class RequestTimingMiddleware
{
    private readonly RequestDelegate next;
    private readonly ILogger logger;

    public RequestTimingMiddleware(
        RequestDelegate next,
        ILogger logger)
    {
        this.next = next;
        this.logger = logger;
    }
    
    public async Task InvokeAsync(HttpContext context)
    {
        var stopWatch = new Stopwatch();

        try
        {
            stopWatch.Start();
            await next(context);
        }
        finally
        {
            stopWatch.Stop();

            var elapsedMilliseconds = stopWatch.ElapsedMilliseconds;
            logger.Information(
                "{RequestMethod} {RequestPath} request took {ElapsedMilliseconds}ms to complete",
                context.Request.Method,
                context.Request.Path,
                elapsedMilliseconds);
        }        
    }
}