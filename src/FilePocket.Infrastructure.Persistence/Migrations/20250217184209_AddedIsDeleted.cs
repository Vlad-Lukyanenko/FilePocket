using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace FilePocket.DataAccess.Migrations
{
    /// <inheritdoc />
    public partial class AddedIsDeleted : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_FilesMetadata_Pockets_PocketId",
                table: "FilesMetadata");

            migrationBuilder.RenameColumn(
                name: "CreatedAt",
                table: "FilesMetadata",
                newName: "CreatedAt");

            migrationBuilder.AddColumn<DateTime>(
                name: "DeletedAt",
                table: "Pockets",
                type: "timestamp with time zone",
                nullable: true);

            migrationBuilder.AddColumn<DateTime>(
                name: "UpdatedAt",
                table: "Pockets",
                type: "timestamp with time zone",
                nullable: true);

            migrationBuilder.AlterColumn<DateTime>(
                name: "UpdatedAt",
                table: "Folders",
                type: "timestamp with time zone",
                nullable: true,
                oldClrType: typeof(DateTime),
                oldType: "timestamp with time zone");

            migrationBuilder.AlterColumn<Guid>(
                name: "PocketId",
                table: "Folders",
                type: "uuid",
                nullable: false,
                defaultValue: new Guid("00000000-0000-0000-0000-000000000000"),
                oldClrType: typeof(Guid),
                oldType: "uuid",
                oldNullable: true);

            migrationBuilder.AddColumn<DateTime>(
                name: "DeletedAt",
                table: "Folders",
                type: "timestamp with time zone",
                nullable: true);

            migrationBuilder.AlterColumn<Guid>(
                name: "PocketId",
                table: "FilesMetadata",
                type: "uuid",
                nullable: false,
                defaultValue: new Guid("00000000-0000-0000-0000-000000000000"),
                oldClrType: typeof(Guid),
                oldType: "uuid",
                oldNullable: true);

            migrationBuilder.AlterColumn<bool>(
                name: "IsDeleted",
                table: "FilesMetadata",
                type: "boolean",
                nullable: false,
                oldClrType: typeof(bool),
                oldType: "boolean",
                oldDefaultValue: false);

            migrationBuilder.AddColumn<DateTime>(
                name: "DeletedAt",
                table: "FilesMetadata",
                type: "timestamp with time zone",
                nullable: true);

            migrationBuilder.AddForeignKey(
                name: "FK_FilesMetadata_Pockets_PocketId",
                table: "FilesMetadata",
                column: "PocketId",
                principalTable: "Pockets",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_FilesMetadata_Pockets_PocketId",
                table: "FilesMetadata");

            migrationBuilder.DropColumn(
                name: "DeletedAt",
                table: "Pockets");

            migrationBuilder.DropColumn(
                name: "UpdatedAt",
                table: "Pockets");

            migrationBuilder.DropColumn(
                name: "DeletedAt",
                table: "Folders");

            migrationBuilder.DropColumn(
                name: "DeletedAt",
                table: "FilesMetadata");

            migrationBuilder.RenameColumn(
                name: "CreatedAt",
                table: "FilesMetadata",
                newName: "CreatedAt");

            migrationBuilder.AlterColumn<DateTime>(
                name: "UpdatedAt",
                table: "Folders",
                type: "timestamp with time zone",
                nullable: false,
                defaultValue: new DateTime(1, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified),
                oldClrType: typeof(DateTime),
                oldType: "timestamp with time zone",
                oldNullable: true);

            migrationBuilder.AlterColumn<Guid>(
                name: "PocketId",
                table: "Folders",
                type: "uuid",
                nullable: true,
                oldClrType: typeof(Guid),
                oldType: "uuid");

            migrationBuilder.AlterColumn<Guid>(
                name: "PocketId",
                table: "FilesMetadata",
                type: "uuid",
                nullable: true,
                oldClrType: typeof(Guid),
                oldType: "uuid");

            migrationBuilder.AlterColumn<bool>(
                name: "IsDeleted",
                table: "FilesMetadata",
                type: "boolean",
                nullable: false,
                defaultValue: false,
                oldClrType: typeof(bool),
                oldType: "boolean");

            migrationBuilder.AddForeignKey(
                name: "FK_FilesMetadata_Pockets_PocketId",
                table: "FilesMetadata",
                column: "PocketId",
                principalTable: "Pockets",
                principalColumn: "Id");
        }
    }
}
