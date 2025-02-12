using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace FilePocket.DataAccess.Migrations
{
    /// <inheritdoc />
    public partial class AddedAccountConsumption : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "AccountSettings");

            migrationBuilder.AlterColumn<string>(
                name: "OriginalName",
                table: "FilesMetadata",
                type: "character varying(255)",
                maxLength: 255,
                nullable: false,
                oldClrType: typeof(string),
                oldType: "text");

            migrationBuilder.AlterColumn<bool>(
                name: "IsDeleted",
                table: "FilesMetadata",
                type: "boolean",
                nullable: false,
                defaultValue: false,
                oldClrType: typeof(bool),
                oldType: "boolean");

            migrationBuilder.AlterColumn<string>(
                name: "ActualName",
                table: "FilesMetadata",
                type: "character varying(255)",
                maxLength: 255,
                nullable: false,
                oldClrType: typeof(string),
                oldType: "text");

            migrationBuilder.CreateTable(
                name: "AccountConsumptions",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uuid", nullable: false),
                    UserId = table.Column<Guid>(type: "uuid", nullable: false),
                    CreatedAt = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
                    UpdatedAt = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
                    IsActivated = table.Column<bool>(type: "boolean", nullable: false, defaultValue: true),
                    MetricType = table.Column<string>(type: "character varying(255)", maxLength: 255, nullable: false),
                    Used = table.Column<double>(type: "double precision", precision: 18, scale: 2, nullable: true, defaultValue: 0.0),
                    Total = table.Column<double>(type: "double precision", precision: 18, scale: 2, nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_AccountConsumptions", x => x.Id);
                    table.ForeignKey(
                        name: "FK_AccountConsumptions_AspNetUsers_UserId",
                        column: x => x.UserId,
                        principalTable: "AspNetUsers",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateIndex(
                name: "IX_Pockets_UserId",
                table: "Pockets",
                column: "UserId");

            migrationBuilder.CreateIndex(
                name: "IX_FilesMetadata_UserId",
                table: "FilesMetadata",
                column: "UserId");

            migrationBuilder.CreateIndex(
                name: "IX_AccountConsumptions_UserId_MetricType",
                table: "AccountConsumptions",
                columns: new[] { "UserId", "MetricType" },
                unique: true);

            migrationBuilder.AddForeignKey(
                name: "FK_FilesMetadata_AspNetUsers_UserId",
                table: "FilesMetadata",
                column: "UserId",
                principalTable: "AspNetUsers",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_Pockets_AspNetUsers_UserId",
                table: "Pockets",
                column: "UserId",
                principalTable: "AspNetUsers",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_FilesMetadata_AspNetUsers_UserId",
                table: "FilesMetadata");

            migrationBuilder.DropForeignKey(
                name: "FK_Pockets_AspNetUsers_UserId",
                table: "Pockets");

            migrationBuilder.DropTable(
                name: "AccountConsumptions");

            migrationBuilder.DropIndex(
                name: "IX_Pockets_UserId",
                table: "Pockets");

            migrationBuilder.DropIndex(
                name: "IX_FilesMetadata_UserId",
                table: "FilesMetadata");

            migrationBuilder.AlterColumn<string>(
                name: "OriginalName",
                table: "FilesMetadata",
                type: "text",
                nullable: false,
                oldClrType: typeof(string),
                oldType: "character varying(255)",
                oldMaxLength: 255);

            migrationBuilder.AlterColumn<bool>(
                name: "IsDeleted",
                table: "FilesMetadata",
                type: "boolean",
                nullable: false,
                oldClrType: typeof(bool),
                oldType: "boolean",
                oldDefaultValue: false);

            migrationBuilder.AlterColumn<string>(
                name: "ActualName",
                table: "FilesMetadata",
                type: "text",
                nullable: false,
                oldClrType: typeof(string),
                oldType: "character varying(255)",
                oldMaxLength: 255);

            migrationBuilder.CreateTable(
                name: "AccountSettings",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uuid", nullable: false),
                    CreatedAt = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
                    StorageCapacity = table.Column<int>(type: "integer", nullable: false),
                    UpdatedAt = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
                    UserId = table.Column<Guid>(type: "uuid", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_AccountSettings", x => x.Id);
                });
        }
    }
}
