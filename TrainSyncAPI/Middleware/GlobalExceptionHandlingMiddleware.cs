using System.Text.Json;
using Microsoft.Data.SqlClient;
using Microsoft.EntityFrameworkCore;

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
            await WriteErrorResponse(context, ex.GetType().Name, "Unauthorized");
        }
        catch (JsonException ex)
        {
            Console.WriteLine($"[GlobalExceptionHandlingMiddleware] JsonException: {ex}");
            _logger.LogWarning(ex, "Invalid JSON received from client.");
            context.Response.StatusCode = StatusCodes.Status400BadRequest;
            await WriteErrorResponse(context, ex.GetType().Name, "Invalid JSON payload");
        }
        catch (BadHttpRequestException ex)
        {
            Console.WriteLine($"[GlobalExceptionHandlingMiddleware] BadHttpRequestException: {ex}");
            _logger.LogWarning(ex, "Bad HTTP request.");
            context.Response.StatusCode = StatusCodes.Status400BadRequest;
            await WriteErrorResponse(context, ex.GetType().Name, "Bad request");
        }
        catch (DbUpdateException ex)
        {
            Console.WriteLine($"[GlobalExceptionHandlingMiddleware] DbUpdateException: {ex}");
            _logger.LogWarning(ex, "Database update exception.");
            if (
                ex.InnerException is SqlException sqlEx
                && (sqlEx.Number == 2601 || sqlEx.Number == 2627)
            )
            {
                // Try to provide a user-friendly, context-specific message
                var message = "A record with the same unique values already exists.";
                var path = context.Request.Path.ToString().ToLower();
                var errorDetails = ex.InnerException?.Message ?? string.Empty;

                if (
                    errorDetails.Contains("IX_exercise_title_user_id") || path.Contains("exercises")
                )
                {
                    message =
                        "You already have an exercise with this title. Exercise titles must be unique per user.";
                }
                // Add more cases here for other unique indexes/entities as needed
                // Example for future: if (errorDetails.Contains("IX_template_title_user_id") || path.Contains("templates"))
                // {
                //     message = "You already have a template with this title. Template titles must be unique per user.";
                // }

                context.Response.StatusCode = StatusCodes.Status409Conflict;
                await WriteErrorResponse(context, "UniqueConstraintViolation", message);
            }
            else
            {
                context.Response.StatusCode = StatusCodes.Status400BadRequest;
                await WriteErrorResponse(context, ex.GetType().Name, "A database error occurred.");
            }
        }
        catch (Exception ex)
        {
            Console.WriteLine($"[GlobalExceptionHandlingMiddleware] Exception: {ex}");
            _logger.LogError(ex, "Unhandled exception occurred.");
            context.Response.StatusCode = StatusCodes.Status500InternalServerError;
            await WriteErrorResponse(context, ex.GetType().Name, "An unexpected error occurred");
        }
    }

    private static Task WriteErrorResponse(HttpContext context, string name, string message)
    {
        context.Response.ContentType = "application/json";
        return context.Response.WriteAsJsonAsync(new { name, message });
    }
}
