using Microsoft.EntityFrameworkCore;
using Modules.Users.Domain;
using Modules.Users.Infrastructure.Database;


namespace API.Host.Seeding;

public class SeedService(
    UserDbContext userContext,
    ILogger<SeedService> logger)
{
    public async Task SeedDataAsync()
    {
        if (await userContext.Users.CountAsync(_ => true) > 0)
        {
            logger.LogInformation("Data already exists, skipping seeding");
            return;
        }

        logger.LogInformation("Starting data seeding...");

        await SeedUsersAsync();

        logger.LogInformation("Data seeding completed");
    }

    private async Task SeedUsersAsync()
    {
        logger.LogInformation("Seeding users...");

        var user = new User
        {
            Id = Guid.NewGuid(),
            Name = "admin",
            Password = "admin",
            Email = "admin@admin.com",
            BirthDate = new DateOnly(2025,4,30)
        };

        await userContext.Users.AddAsync(user);
        await userContext.SaveChangesAsync();
    }
}