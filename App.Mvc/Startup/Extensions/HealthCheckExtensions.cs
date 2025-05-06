using Microsoft.AspNetCore.Diagnostics.HealthChecks;
using Microsoft.Extensions.Diagnostics.HealthChecks;
using System.Net.Mime;
using System.Text.Json;
namespace App.MVC.Startup.Extensions
{
    public static class HealthCheckExtensions
    {

        public static WebApplicationBuilder AddHealthCheckConfiguration(this WebApplicationBuilder builder)
        {
            builder.Services.AddHealthChecks()
            .AddProcessAllocatedMemoryHealthCheck(
                512,
                name: "Memory",
                failureStatus: HealthStatus.Unhealthy,
                tags: new string[] { "Memory" });
            return builder;
        }
        public static WebApplication UseHealthCheckConfiguration(this WebApplication app)
        {
            return (WebApplication)app.UseHealthChecks("/status",
              new HealthCheckOptions()
              {
                  ResponseWriter = async (context, report) =>
                  {
                      var result = JsonSerializer.Serialize(
                             new
                             {
                                 currentTime = DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss"),
                                 statusApplication = report.Status.ToString(),
                                 monitors = report.Entries.Select(e => new { key = e.Key, value = Enum.GetName(typeof(HealthStatus), e.Value.Status) })
                             });

                      context.Response.ContentType = MediaTypeNames.Application.Json;
                      await context.Response.WriteAsync(result);
                  }
              });
        }

    }
}
