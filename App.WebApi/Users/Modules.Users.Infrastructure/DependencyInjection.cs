using Microsoft.Extensions.DependencyInjection;
using Modules.Users.Infrastructure.Database;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Modules.Users.Infrastructure.Database.Repositories.Interfaces;
using Modules.Users.Infrastructure.Database.Repositories;

namespace Modules.Users.Infrastructure
{
    public static class DependencyInjection
    {
        public static IServiceCollection AddUsersInfrastructure(this IServiceCollection services, IConfiguration configuration)
        {
            var sqlServerConnectionString = configuration.GetConnectionString("SqlServer");
            services.AddDbContext<UserDbContext>(x => x
                    .UseSqlServer(sqlServerConnectionString, options =>
                    options.MigrationsHistoryTable(DbConsts.MigrationHistoryTableName, DbConsts.UsersSchemaName)
                    .MigrationsAssembly("Modules.Users.Infrastructure"))
                    .UseSnakeCaseNamingConvention()
                    );
            services.AddScoped<IUserRepository, UserRepository>();
            

            return services;
        }
    }
}
