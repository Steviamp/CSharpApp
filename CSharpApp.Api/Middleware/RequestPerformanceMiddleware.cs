using System.Diagnostics;

namespace CSharpApp.Api.Middleware
{
    public class RequestPerformanceMiddleware
    {
        private readonly RequestDelegate _next;
        private readonly ILogger<RequestPerformanceMiddleware> _logger;

        public RequestPerformanceMiddleware(RequestDelegate next, ILogger<RequestPerformanceMiddleware> logger)
        {
            _next = next;
            _logger = logger;
        }

        public async Task InvokeAsync(HttpContext context)
        {
            var stopwatch = Stopwatch.StartNew();
            var requestPath = context.Request.Path;
            var method = context.Request.Method;

            try
            {
                await _next(context);
            }
            finally
            {
                stopwatch.Stop();
                var statusCode = context.Response.StatusCode;
                var elapsed = stopwatch.ElapsedMilliseconds;

                var performanceLog = new
                {
                    Timestamp = DateTime.UtcNow,
                    Path = requestPath,
                    Method = method,
                    StatusCode = statusCode,
                    DurationMs = elapsed,
                    ClientIP = context.Connection.RemoteIpAddress?.ToString(),
                    UserAgent = context.Request.Headers.UserAgent.ToString()
                };

                var logLevel = DetermineLogLevel(elapsed, statusCode);

                _logger.Log(logLevel,
                    "Request {Method} {Path} completed in {Duration}ms with status {StatusCode}",
                    method, requestPath, elapsed, statusCode);

                // Log detailed information at debug level
                _logger.LogDebug(
                    "Detailed request performance: {@PerformanceLog}",
                    performanceLog);
            }
        }

        private static LogLevel DetermineLogLevel(long durationMs, int statusCode)
        {
            // Error responses
            if (statusCode >= 500)
            {
                return LogLevel.Error;
            }

            // Client errors
            if (statusCode >= 400)
            {
                return LogLevel.Warning;
            }

            // Slow requests (over 500ms)
            if (durationMs > 500)
            {
                return LogLevel.Warning;
            }

            return LogLevel.Information;
        }
    }

    // Extension method to make it easier to add the middleware
    public static class RequestPerformanceMiddlewareExtensions
    {
        public static IApplicationBuilder UseRequestPerformance(
            this IApplicationBuilder builder)
        {
            return builder.UseMiddleware<RequestPerformanceMiddleware>();
        }
    }
}
