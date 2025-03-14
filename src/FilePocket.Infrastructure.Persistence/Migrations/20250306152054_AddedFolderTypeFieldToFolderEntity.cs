using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace FilePocket.DataAccess.Migrations
{
    /// <inheritdoc />
    public partial class AddedFolderTypeFieldToFolderEntity : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<int>(
                name: "FolderType",
                table: "Folders",
                type: "integer",
                nullable: false,
                defaultValue: 0);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "FolderType",
                table: "Folders");
        }
    }
}
