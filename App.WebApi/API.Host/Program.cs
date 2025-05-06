using API.Host.Configurations;
using API.Host.Extensions;
using API.Host.Seeding;
using Carter;
using FluentValidation;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using Modules.Books.Features;
using Modules.Books.Infrastructure;
using Modules.Books.Infrastructure.Database;
using Modules.Users.Features;
using Modules.Users.Infrastructure;
using Modules.Users.Infrastructure.Database;
using Prometheus;

var builder = WebApplication.CreateBuilder(args);

builder.AddHostLogging();
AuthenticationConfiguration.AddAuthenticationConfiguration(builder);

builder.Services.AddWebHostInfrastructure();

builder.Services.AddUsersModule(builder.Configuration)
    .AddUsersInfrastructure(builder.Configuration);

builder.Services.AddBooksModule(builder.Configuration)
    .AddBooksInfrastructure(builder.Configuration);

HealthCheckExtensions.AddHealthCheckConfiguration(builder);
RequestsExtensions.AddProblemDetailsConfiguration(builder);

var app = builder.Build();

using (var scope = app.Services.CreateScope())
{
    var dbContext = scope.ServiceProvider.GetRequiredService<UserDbContext>();
    await dbContext.Database.MigrateAsync();

    var booksdbContext = scope.ServiceProvider.GetRequiredService<BooksDbContext>();
    await booksdbContext.Database.MigrateAsync();

    var seedService = scope.ServiceProvider.GetRequiredService<SeedService>();
    await seedService.SeedDataAsync();
}

if (app.Environment.IsDevelopment())
{
    app.MapOpenApi();
}

app.UseHttpsRedirection();

if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

HealthCheckExtensions.UseHealthCheckConfiguration(app);

app.UseMetricServer();
app.UseHttpMetrics();

app.UseAuthentication();
app.UseAuthorization();

app.MapCarter();
app.Run();