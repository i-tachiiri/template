using Microsoft.Azure.Functions.Worker;
using Microsoft.Azure.Functions.Worker.Middleware;
using Microsoft.Extensions.Logging;
using Shared;
using System;
using System.Threading.Tasks;

public sealed class ExceptionLoggingMiddleware : IFunctionsWorkerMiddleware
{
    private readonly ILogger<ExceptionLoggingMiddleware> _logger;

    public ExceptionLoggingMiddleware(ILogger<ExceptionLoggingMiddleware> logger)
    {
        _logger = logger;
    }

    public async Task Invoke(FunctionContext context, FunctionExecutionDelegate next)
    {
        try
        {
            await next(context);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Unhandled exception");
            using var activity = Telemetry.StartActivity("UnhandledException");
            activity?.SetTag("exception.type", ex.GetType().FullName);
            activity?.SetTag("exception.message", ex.Message);
            activity?.SetTag("exception.stacktrace", ex.StackTrace);
            throw;
        }
    }
}
