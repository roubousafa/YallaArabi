using Microsoft.OpenApi.Any;
using Microsoft.OpenApi.Models;
using Swashbuckle.AspNetCore.SwaggerGen;

namespace YallaArabi.Api.Common;

// Documents the optional X-Time-Zone header on every endpoint so the app team sees it in Swagger.
public class TimeZoneHeaderOperationFilter : IOperationFilter
{
    public void Apply(OpenApiOperation operation, OperationFilterContext context)
    {
        operation.Parameters ??= new List<OpenApiParameter>();

        operation.Parameters.Add(new OpenApiParameter
        {
            Name = "X-Time-Zone",
            In = ParameterLocation.Header,
            Required = false,
            Description = "Student's IANA time zone (e.g. Europe/Berlin). Defines 'today' for streak and daily goal. Defaults to UTC.",
            Schema = new OpenApiSchema { Type = "string", Example = new OpenApiString("Asia/Damascus") }
        });
    }
}
