using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace FilePocket.DataAccess.Migrations
{
    /// <inheritdoc />
    public partial class UpdatedFileData : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_FilesMetadata_Pockets_PocketId",
                table: "FilesMetadata");

            migrationBuilder.AlterColumn<Guid>(
                name: "PocketId",
                table: "FilesMetadata",
                type: "uuid",
                nullable: true,
                oldClrType: typeof(Guid),
                oldType: "uuid");

            migrationBuilder.AddForeignKey(
                name: "FK_FilesMetadata_Pockets_PocketId",
                table: "FilesMetadata",
                column: "PocketId",
                principalTable: "Pockets",
                principalColumn: "Id");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_FilesMetadata_Pockets_PocketId",
                table: "FilesMetadata");

            migrationBuilder.AlterColumn<Guid>(
                name: "PocketId",
                table: "FilesMetadata",
                type: "uuid",
                nullable: false,
                defaultValue: new Guid("00000000-0000-0000-0000-000000000000"),
                oldClrType: typeof(Guid),
                oldType: "uuid",
                oldNullable: true);

            migrationBuilder.AddForeignKey(
                name: "FK_FilesMetadata_Pockets_PocketId",
                table: "FilesMetadata",
                column: "PocketId",
                principalTable: "Pockets",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);
        }
    }
}
