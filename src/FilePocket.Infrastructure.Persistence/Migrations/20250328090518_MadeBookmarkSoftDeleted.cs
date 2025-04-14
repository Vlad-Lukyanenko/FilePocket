using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace FilePocket.DataAccess.Migrations
{
    /// <inheritdoc />
    public partial class MadeBookmarkSoftDeleted : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<DateTime>(
                name: "DeletedAt",
                table: "Bookmarks",
                type: "timestamp with time zone",
                nullable: true);

            migrationBuilder.AddColumn<bool>(
                name: "IsDeleted",
                table: "Bookmarks",
                type: "boolean",
                nullable: false,
                defaultValue: false);

            migrationBuilder.CreateIndex(
                name: "IX_Folders_PocketId",
                table: "Folders",
                column: "PocketId");

            migrationBuilder.CreateIndex(
                name: "IX_FilesMetadata_FolderId",
                table: "FilesMetadata",
                column: "FolderId");

            migrationBuilder.AddForeignKey(
                name: "FK_FilesMetadata_Folders_FolderId",
                table: "FilesMetadata",
                column: "FolderId",
                principalTable: "Folders",
                principalColumn: "Id");

            migrationBuilder.AddForeignKey(
                name: "FK_Folders_Pockets_PocketId",
                table: "Folders",
                column: "PocketId",
                principalTable: "Pockets",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_FilesMetadata_Folders_FolderId",
                table: "FilesMetadata");

            migrationBuilder.DropForeignKey(
                name: "FK_Folders_Pockets_PocketId",
                table: "Folders");

            migrationBuilder.DropIndex(
                name: "IX_Folders_PocketId",
                table: "Folders");

            migrationBuilder.DropIndex(
                name: "IX_FilesMetadata_FolderId",
                table: "FilesMetadata");

            migrationBuilder.DropColumn(
                name: "DeletedAt",
                table: "Bookmarks");

            migrationBuilder.DropColumn(
                name: "IsDeleted",
                table: "Bookmarks");
        }
    }
}
