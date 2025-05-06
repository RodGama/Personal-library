using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.IdentityModel.Tokens;
using System.Text;

namespace App.MVC.Startup.Configurations
{
    public static class AuthenticationConfiguration
    {
        public static WebApplicationBuilder AddAuthenticationConfiguration(this WebApplicationBuilder builder)
        {
            var key = Encoding.ASCII.GetBytes(Environment.GetEnvironmentVariable("ASPNETCORE_AUTO_RELOAD_WS_KEY"));


            builder.Services.AddAuthentication(x =>
            {
                x.DefaultAuthenticateScheme = JwtBearerDefaults.AuthenticationScheme;
                x.DefaultChallengeScheme = JwtBearerDefaults.AuthenticationScheme;
            })
            .AddJwtBearer(x =>
            {
                x.RequireHttpsMetadata = false;
                x.SaveToken = true;
                x.TokenValidationParameters = new TokenValidationParameters
                {
                    ValidateIssuerSigningKey = true,
                    IssuerSigningKey = new SymmetricSecurityKey(key),
                    ValidateIssuer = false,
                    ValidateAudience = false
                };
            });

            builder.Services.AddAuthorization();
            return builder;
        }
    }
    
}
