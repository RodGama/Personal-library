using Carter;
using FluentValidation;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;

namespace Modules.Books.Features
{
    public static class DependencyInjection
    {
        public static IServiceCollection AddBooksModule(this IServiceCollection services, IConfiguration configuration)
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