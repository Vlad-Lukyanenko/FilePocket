using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace FilePocket.DataAccess.Migrations
{
    /// <inheritdoc />
    public partial class CascadeDeleteFilesInFolder : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_FilesMetadata_Folders_FolderId",
                table: "FilesMetadata");

            migrationBuilder.AddForeignKey(
                name: "FK_FilesMetadata_Folders_FolderId",
                table: "FilesMetadata",
                column: "FolderId",
                principalTable: "Folders",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_FilesMetadata_Folders_FolderId",
                table: "FilesMetadata");

            migrationBuilder.RenameColumn(
                name: "DateCreated",
                table: "Pockets",
                newName: "CreatedAt");

            migrationBuilder.AddForeignKey(
                name: "FK_FilesMetadata_Folders_FolderId",
                table: "FilesMetadata",
                column: "FolderId",
                principalTable: "Folders",
                principalColumn: "Id");
        }
    }
}
