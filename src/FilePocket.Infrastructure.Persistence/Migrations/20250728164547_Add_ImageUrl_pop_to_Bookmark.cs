using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace FilePocket.Infrastructure.Persistence.Migrations
{
    /// <inheritdoc />
    public partial class Add_ImageUrl_pop_to_Bookmark : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<string>(
                name: "ImageUrl",
                table: "Bookmarks",
                type: "text",
                nullable: false,
                defaultValue: "");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "ImageUrl",
                table: "Bookmarks");
        }
    }
}
