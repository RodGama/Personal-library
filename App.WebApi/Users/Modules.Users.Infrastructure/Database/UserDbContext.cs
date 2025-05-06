using Microsoft.EntityFrameworkCore;
using Modules.Users.Domain;

namespace Modules.Users.Infrastructure.Database
{

    public class UserDbContext : DbContext
    {
        public UserDbContext(DbContextOptions<UserDbContext> options)
            : base(options)
        {
        }
        public UserDbContext()
        {
        }
        public DbSet<User> Users { get; set; }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);

            modelBuilder.HasDefaultSchema(DbConsts.UsersSchemaName);

            modelBuilder.Entity<User>(entity =>
            {
                entity.HasKey(x => x.Id);
                entity.HasIndex(x => x.Email);

                entity.Property(x => x.Email).IsRequired();
                entity.Property(x => x.Name).IsRequired();
                entity.Property(x => x.Password).IsRequired();
                entity.Property(x => x.BirthDate).IsRequired();

                //entity.HasMany(x => x.Books)
                //    .WithOne(x => x.User)
                //    .HasForeignKey(x => x.UserId);
            });

        }
    }
}
