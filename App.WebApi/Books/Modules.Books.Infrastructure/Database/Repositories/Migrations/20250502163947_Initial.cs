using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Modules.Books.Infrastructure.Migrations
{
    /// <inheritdoc />
    public partial class Initial : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.EnsureSchema(
                name: "books");

            migrationBuilder.CreateTable(
                name: "books",
                schema: "books",
                columns: table => new
                {
                    id = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    title = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    author = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    description = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    publisher = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    genre = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    image_base64 = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    isbn = table.Column<int>(type: "int", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("pk_books", x => x.id);
                });

            migrationBuilder.CreateIndex(
                name: "ix_books_isbn",
                schema: "books",
                table: "books",
                column: "isbn");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "books",
                schema: "books");
        }
    }
}
