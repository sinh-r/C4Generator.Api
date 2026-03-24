using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace C4Generator.Infrastructure.Persistence.Migrations
{
    /// <inheritdoc />
    public partial class AddSourceControlFields : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<string>(
                name: "external_id",
                table: "repositories",
                type: "character varying(100)",
                maxLength: 100,
                nullable: false,
                defaultValue: "");

            migrationBuilder.AddColumn<DateTime>(
                name: "last_synced_at",
                table: "repositories",
                type: "timestamp with time zone",
                nullable: false,
                defaultValue: new DateTime(1, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified));

            migrationBuilder.AddColumn<int>(
                name: "provider",
                table: "repositories",
                type: "integer",
                nullable: false,
                defaultValue: 1);

            migrationBuilder.CreateIndex(
                name: "IX_repositories_provider_external_id",
                table: "repositories",
                columns: new[] { "provider", "external_id" },
                unique: true);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropIndex(
                name: "IX_repositories_provider_external_id",
                table: "repositories");

            migrationBuilder.DropColumn(
                name: "external_id",
                table: "repositories");

            migrationBuilder.DropColumn(
                name: "last_synced_at",
                table: "repositories");

            migrationBuilder.DropColumn(
                name: "provider",
                table: "repositories");
        }
    }
}
