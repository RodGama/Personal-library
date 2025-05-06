using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Modules.Books.Infrastructure.Migrations
{
    /// <inheritdoc />
    public partial class isbn : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AlterColumn<long>(
                name: "isbn",
                schema: "books",
                table: "books",
                type: "bigint",
                nullable: false,
                oldClrType: typeof(int),
                oldType: "int");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AlterColumn<int>(
                name: "isbn",
                schema: "books",
                table: "books",
                type: "int",
                nullable: false,
                oldClrType: typeof(long),
                oldType: "bigint");
        }
    }
}
