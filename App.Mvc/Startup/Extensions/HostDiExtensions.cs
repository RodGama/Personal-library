using Microsoft.AspNetCore.Http.Json;
using Microsoft.Extensions.DependencyInjection;
using System.Text.Json.Serialization;
using Serilog;
using Serilog.Sinks.SystemConsole;
namespace App.MVC.Startup.Extensions
{
    public static class HostDiExtensions
    {
        public static IServiceCollection AddWebHostInfrastructure(this IServiceCollection services)
        {

            services.Configure<JsonOptions>(opt =>
            {
                opt.SerializerOptions.Converters.Add(new JsonStringEnumConverter());
            });

            return services;
        }

        public static void AddHostLogging(this WebApplicationBuilder builder)
        {
            //builder.Host.UseSerilog((context, loggerConfig) =>
            //    loggerConfig.ReadFrom.Configuration(context.Configuration));
        }
    }
}
