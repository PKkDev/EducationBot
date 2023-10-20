using Microsoft.OpenApi.Any;
using Microsoft.OpenApi.Models;
using Swashbuckle.AspNetCore.SwaggerGen;

namespace EducationBot.Service.API.AuthRules;

public class AddRequiredHeaderParameter : IOperationFilter
{
    private readonly IConfiguration _configuration;
    private readonly IHostEnvironment _env;

    public AddRequiredHeaderParameter(IConfiguration configuration, IHostEnvironment env)
    {
        _configuration = configuration;
        _env = env;
    }

    public void Apply(OpenApiOperation operation, OperationFilterContext context)
    {
        operation.Parameters ??= new List<OpenApiParameter>();

        operation.Parameters.Add(new OpenApiParameter
        {
            Name = _configuration["ApiKeyHeader"],
            In = ParameterLocation.Header,
            Required = false,
            Schema = _env.IsDevelopment()
            ? new OpenApiSchema
            {
                Type = "String",
                Default = new OpenApiString(_configuration["ApiKey"])
            }
            : null
        });
    }
}
