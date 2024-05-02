using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Shortener.Migrations.Migrations
{
    /// <inheritdoc />
    public partial class AddedUniqueIndexAndSoftDelete : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<bool>(
                name: "IsDeleted",
                table: "UrlPairs",
                type: "boolean",
                nullable: false,
                defaultValue: false);

            migrationBuilder.CreateIndex(
                name: "IX_UrlPairs_ShortUrl",
                table: "UrlPairs",
                column: "ShortUrl",
                unique: true);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropIndex(
                name: "IX_UrlPairs_ShortUrl",
                table: "UrlPairs");

            migrationBuilder.DropColumn(
                name: "IsDeleted",
                table: "UrlPairs");
        }
    }
}
