using System.Net.Mime;
using System.Text.Json.Serialization;

namespace EducationBot.Service.API.Middleware;

public class ErrorHandler
{
    private readonly RequestDelegate _next;

    public ErrorHandler(RequestDelegate next)
    {
        _next = next;
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
            context.Response.StatusCode = StatusCodes.Status400BadRequest;

            var message = ex.Message;

            await context.Response.WriteAsJsonAsync(new ApiErrorResponse("Api Error", message));
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
