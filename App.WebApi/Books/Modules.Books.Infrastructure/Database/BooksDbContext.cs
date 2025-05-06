using Microsoft.EntityFrameworkCore;
using Modules.Books.Domain;
using System.Collections.Generic;

namespace Modules.Books.Infrastructure.Database
{

    public class BooksDbContext : DbContext
    {
        public BooksDbContext(DbContextOptions<BooksDbContext> options)
            : base(options)
        {
        }
        public DbSet<Book> Books { get; set; }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);

            modelBuilder.HasDefaultSchema(DbConsts.BooksSchemaName);

            modelBuilder.Entity<Book>(entity =>
            {
                entity.HasKey(x => x.Id);
                entity.HasIndex(x => x.ISBN);

                entity.Property(x => x.Title).IsRequired();
                entity.Property(x => x.Author).IsRequired();
                entity.Property(x => x.Publisher).IsRequired();
                entity.Property(x => x.Description).IsRequired();
                entity.Property(x => x.Genre).IsRequired();
                entity.Property(x => x.ImageBase64).IsRequired();
                entity.Property(x => x.UserId).IsRequired();
            });

        }
    }
}
