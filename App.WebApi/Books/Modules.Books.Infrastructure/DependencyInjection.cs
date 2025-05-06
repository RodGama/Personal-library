using Microsoft.Extensions.DependencyInjection;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Modules.Books.Infrastructure.Database;
using Modules.Books.Infrastructure.Database.Repositories.Interfaces;
using Modules.Books.Infrastructure.Database.Repositories;

namespace Modules.Books.Infrastructure
{
    public static class DependencyInjection
    {
        public static IServiceCollection AddBooksInfrastructure(this IServiceCollection services, IConfiguration configuration)
        {
            var sqlServerConnectionString = configuration.GetConnectionString("SqlServer");
            services.AddDbContext<BooksDbContext>(x => x
                    .UseSqlServer(sqlServerConnectionString, options =>
                    options.MigrationsHistoryTable(DbConsts.MigrationHistoryTableName, DbConsts.BooksSchemaName)
                    .MigrationsAssembly("Modules.Books.Infrastructure"))
                    .UseSnakeCaseNamingConvention()
                    );

            services.AddTransient<IBookRepository, BookRepository>();
            

            return services;
        }
    }
}
