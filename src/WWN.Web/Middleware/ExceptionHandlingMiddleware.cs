using System.Text.Json;
using System.Text.Json.Serialization;

namespace WWN.Web.Middleware;

public class ExceptionHandlingMiddleware
{
    private readonly RequestDelegate _next;
    private readonly ILogger<ExceptionHandlingMiddleware> _logger;
    private readonly IWebHostEnvironment _env;

    public ExceptionHandlingMiddleware(RequestDelegate next, ILogger<ExceptionHandlingMiddleware> logger, IWebHostEnvironment env)
    {
        _next = next;
        _logger = logger;
        _env = env;
    }

    public async Task InvokeAsync(HttpContext context)
    {
        try
        {
            await _next(context);
        }
        catch (Exception ex)
        {
            await HandleExceptionAsync(context, ex);
        }
    }

    private async Task HandleExceptionAsync(HttpContext context, Exception exception)
    {
        var (statusCode, title) = exception switch
        {
            KeyNotFoundException => (StatusCodes.Status404NotFound, "Not Found"),
            InvalidOperationException => (StatusCodes.Status409Conflict, "Conflict"),
            ArgumentException => (StatusCodes.Status400BadRequest, "Bad Request"),
            _ => (StatusCodes.Status500InternalServerError, "Internal Server Error")
        };

        var traceId = context.TraceIdentifier;
        _logger.LogError(exception, "Unhandled exception. TraceId: {TraceId}", traceId);

        context.Response.StatusCode = statusCode;
        context.Response.ContentType = "application/problem+json";

        var isDev = _env.IsDevelopment();
        var problem = new ProblemResponse(
            Type: $"https://httpstatuses.com/{statusCode}",
            Title: title,
            Status: statusCode,
            Detail: (statusCode == StatusCodes.Status500InternalServerError && !isDev)
                ? "An unexpected error occurred."
                : exception.Message,
            TraceId: traceId,
            StackTrace: isDev ? exception.StackTrace : null
        );

        var options = new JsonSerializerOptions
        {
            PropertyNamingPolicy = JsonNamingPolicy.CamelCase,
            DefaultIgnoreCondition = JsonIgnoreCondition.WhenWritingNull
        };

        await context.Response.WriteAsync(JsonSerializer.Serialize(problem, options));
    }

    private record ProblemResponse(
        string Type,
        string Title,
        int Status,
        string Detail,
        string TraceId,
        string? StackTrace
    );
}
