using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace FilePocket.DataAccess.Migrations
{
    /// <inheritdoc />
    public partial class AddUpdatedAtPropToFileMetadata : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<DateTime>(
                name: "UpdatedAt",
                table: "FilesMetadata",
                type: "timestamp with time zone",
                nullable: true);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "UpdatedAt",
                table: "FilesMetadata");
        }
    }
}
