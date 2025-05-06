using Microsoft.AspNetCore.Cors;
namespace API.Host.Configurations
{
    public static class CorsConfiguration
    {
        // Configuração do Builder
        public static WebApplicationBuilder AddCorsConfiguration(this WebApplicationBuilder builder)
        {
            builder.Services.AddCors(options =>
            {
                options.AddPolicy("AllowAll", builder =>
                    builder.AllowAnyOrigin().AllowAnyMethod().AllowAnyHeader());
            });
            return builder;
        }

        //Configuração da aplicação(Middlewares)
        public static WebApplication UseCorsConfiguration(this WebApplication app)
        {
            return (WebApplication)app.UseCors("AllowAll");
        }
    }
}
