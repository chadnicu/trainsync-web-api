namespace TrainSyncAPI.Middleware;

public class GlobalExceptionHandlingMiddleware
{
    private readonly ILogger<GlobalExceptionHandlingMiddleware> _logger;
    private readonly RequestDelegate _next;

    public GlobalExceptionHandlingMiddleware(
        RequestDelegate next,
        ILogger<GlobalExceptionHandlingMiddleware> logger
    )
    {
        _next = next;
        _logger = logger;
    }

    public async Task Invoke(HttpContext context)
    {
        try
        {
            Console.WriteLine($"[Request] {context.Request.Method} {context.Request.Path}");
            if (context.Request.Headers.ContainsKey("Authorization"))
                Console.WriteLine(
                    $"[Request] Authorization: {context.Request.Headers["Authorization"]}"
                );
            await _next(context);
        }
        catch (UnauthorizedAccessException ex)
        {
            Console.WriteLine(
                $"[GlobalExceptionHandlingMiddleware] UnauthorizedAccessException: {ex}"
            );
            _logger.LogWarning(ex, "Unauthorized access.");
            context.Response.StatusCode = StatusCodes.Status401Unauthorized;
            await context.Response.WriteAsJsonAsync(new { error = "Unauthorized" });
        }
        catch (Exception ex)
        {
            Console.WriteLine($"[GlobalExceptionHandlingMiddleware] Exception: {ex}");
            _logger.LogError(ex, "Unhandled exception occurred.");
            context.Response.StatusCode = StatusCodes.Status500InternalServerError;
            await context.Response.WriteAsJsonAsync(new { error = "An unexpected error occurred" });
        }
    }
}
