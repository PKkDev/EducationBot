using Microsoft.AspNetCore.Authorization;

namespace EducationBot.Service.API.AuthRules;

public class ApiKeyHandler : AuthorizationHandler<ApiKeyRequirement>
{
    private readonly IHttpContextAccessor _httpContextAccessor;
    private readonly IConfiguration _configuration;

    public ApiKeyHandler(IHttpContextAccessor httpContextAccessor, IConfiguration configuration)
    {
        _httpContextAccessor = httpContextAccessor;
        _configuration = configuration;

        // string encodedString = Convert.ToBase64String(System.Text.Encoding.UTF8.GetBytes(str));
        // string decodedString = System.Text.Encoding.UTF8.GetString(Convert.FromBase64String(encodedString));
    }

    protected override Task HandleRequirementAsync(AuthorizationHandlerContext context, ApiKeyRequirement requirement)
    {
        var key = _configuration["ApiKeyHeader"];
        string? apiKey = _httpContextAccessor?.HttpContext?.Request.Headers[key].ToString();

        if (!IsValidApiKey(apiKey))
        {
            context.Fail();
            return Task.CompletedTask;
        }

        context.Succeed(requirement);
        return Task.CompletedTask;
    }

    private bool IsValidApiKey(string userApiKey)
    {
        if (string.IsNullOrWhiteSpace(userApiKey))
            return false;

        string? apiKey = _configuration["ApiKey"];
        if (apiKey == null || apiKey != userApiKey)
            return false;

        return true;
    }
}
