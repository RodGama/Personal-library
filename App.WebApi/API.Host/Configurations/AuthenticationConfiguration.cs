using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.OpenApi;
using Microsoft.EntityFrameworkCore.Metadata.Internal;
using Microsoft.IdentityModel.Tokens;
using Microsoft.OpenApi.Models;
using System.Reflection;
using System.Text;

namespace API.Host.Configurations
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

            builder.Services.AddOpenApi(options =>
            {
                options.AddDocumentTransformer<BearerSecuritySchemeTransformer>();
            });

            builder.Services.AddSwaggerGen(c =>
            {
                c.SwaggerDoc("v1", new OpenApiInfo { Title = "Your API", Version = "v1" });
                var xmlFile = $"{Assembly.GetExecutingAssembly().GetName().Name}.xml";
                c.AddSecurityDefinition("Bearer", new OpenApiSecurityScheme
                {
                    Description = "JWT Authorization header",
                    Name = "Authorization",
                    In = ParameterLocation.Header,
                    Type = SecuritySchemeType.ApiKey,
                    Scheme = "Bearer"
                });

                c.AddSecurityRequirement(new OpenApiSecurityRequirement
                {
                    {
                        new OpenApiSecurityScheme
                        {
                            Reference = new OpenApiReference
                            {
                                Type = ReferenceType.SecurityScheme,
                                Id = "Bearer"
                            }
                        },
                        new List<string>()
                    }
                });
            });

            return builder;
        }
    }
    internal sealed class BearerSecuritySchemeTransformer(IAuthenticationSchemeProvider authenticationSchemeProvider) : IOpenApiDocumentTransformer
    {
        public async Task TransformAsync(OpenApiDocument document, OpenApiDocumentTransformerContext context, CancellationToken cancellationToken)
        {
            var authenticationSchemes = await authenticationSchemeProvider.GetAllSchemesAsync();
            if (authenticationSchemes.Any(authScheme => authScheme.Name == "Bearer"))
            {
                var requirements = new Dictionary<string, OpenApiSecurityScheme>
                {
                    ["Bearer"] = new OpenApiSecurityScheme
                    {
                        Type = SecuritySchemeType.Http,
                        Scheme = "bearer",
                        In = ParameterLocation.Header,
                        BearerFormat = "Json Web Token"
                    }
                };
                document.Components ??= new OpenApiComponents();
                document.Components.SecuritySchemes = requirements;
            }
        }
    }
}
