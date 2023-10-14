using System.Net.Mime;
using System.Text.Json.Serialization;

namespace EducationBot.Service.API.Middleware;

public class ErrorHandler
{
    private readonly RequestDelegate _next;
    private readonly ILogger<ErrorHandler> _logger;

    public ErrorHandler(RequestDelegate next, ILogger<ErrorHandler> logger)
    {
        _next = next;
        _logger = logger;
    }

    public async Task Invoke(HttpContext context)
    {
        try
        {
            if (context.Request.ContentType == MediaTypeNames.Application.Json)
                context.Request.EnableBuffering();

            await _next(context);
        }
        catch (TaskCanceledException)
        {
            // Нормальное поведение
        }
        catch (OperationCanceledException)
        {
            // Нормальное поведение
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Api error");

            context.Response.StatusCode = StatusCodes.Status400BadRequest;

            var message = ex.Message;

            await context.Response.WriteAsJsonAsync(new ApiErrorResponse("Api error", message));
        }
    }
}

public class ApiErrorResponse
{
    [JsonPropertyName("errorCodeName")]
    public string ErrorCodeName { get; set; }

    [JsonPropertyName("message")]
    public string Message { get; set; }

    public ApiErrorResponse(string errorCodeName, string message)
    {
        ErrorCodeName = errorCodeName;
        Message = message;
    }
}
