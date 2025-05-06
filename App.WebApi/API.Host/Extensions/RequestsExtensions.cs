using Microsoft.AspNetCore.Http.Features;
using Microsoft.Extensions.Diagnostics.HealthChecks;

namespace API.Host.Extensions
{
    public static class RequestsExtensions
    {
        public static WebApplicationBuilder AddProblemDetailsConfiguration(this WebApplicationBuilder builder)
        {
            builder.Services.AddProblemDetails(options =>
            {
                options.CustomizeProblemDetails = context =>
                {
                    context.ProblemDetails.Instance =
                       $"{context.HttpContext.Request.Method} {context.HttpContext.Request.Path}";
                    context.ProblemDetails.Extensions.TryAdd("requestId", context.HttpContext.TraceIdentifier);

                    var activity = context.HttpContext.Features.Get<IHttpActivityFeature>()?.Activity;
                    context.ProblemDetails.Extensions.TryAdd("traceId", activity?.Id);
                };
            });
            return builder;
        }
    }
}
