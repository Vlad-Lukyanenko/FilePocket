using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace FilePocket.DataAccess.Migrations
{
    /// <inheritdoc />
    public partial class CascadeDeleteBokmarksOnFolder : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Bookmarks_Folders_FolderId",
                table: "Bookmarks");

            migrationBuilder.AddForeignKey(
                name: "FK_Bookmarks_Folders_FolderId",
                table: "Bookmarks",
                column: "FolderId",
                principalTable: "Folders",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Bookmarks_Folders_FolderId",
                table: "Bookmarks");

            migrationBuilder.AddForeignKey(
                name: "FK_Bookmarks_Folders_FolderId",
                table: "Bookmarks",
                column: "FolderId",
                principalTable: "Folders",
                principalColumn: "Id");
        }
    }
}
