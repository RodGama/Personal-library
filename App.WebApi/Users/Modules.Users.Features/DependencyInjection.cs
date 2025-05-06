using Carter;
using FluentValidation;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Modules.Common.Features;

namespace Modules.Users.Features
{
    public static class DependencyInjection
    {
        public static IServiceCollection AddUsersModule(this IServiceCollection services, IConfiguration configuration)
        {
            services.AddMediatR(config =>
            {
                config.RegisterServicesFromAssembly(typeof(DependencyInjection).Assembly);
            });

            services.AddValidatorsFromAssembly(typeof(DependencyInjection).Assembly);

            return services;
        }
    }
}