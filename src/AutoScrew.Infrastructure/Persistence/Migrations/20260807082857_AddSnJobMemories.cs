using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace AutoScrew.Infrastructure.Persistence.Migrations
{
    /// <inheritdoc />
    public partial class AddSnJobMemories : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "SnJobMemories",
                columns: table => new
                {
                    SerialNumber = table.Column<string>(type: "TEXT", nullable: false),
                    PartNumber = table.Column<string>(type: "TEXT", nullable: false),
                    Status = table.Column<int>(type: "INTEGER", nullable: false),
                    PayloadJson = table.Column<string>(type: "TEXT", nullable: false),
                    UpdatedAt = table.Column<DateTimeOffset>(type: "TEXT", nullable: false),
                    CompletedAt = table.Column<DateTimeOffset>(type: "TEXT", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_SnJobMemories", x => x.SerialNumber);
                });

            migrationBuilder.CreateIndex(
                name: "IX_SnJobMemories_Status",
                table: "SnJobMemories",
                column: "Status");

            migrationBuilder.CreateIndex(
                name: "IX_SnJobMemories_UpdatedAt",
                table: "SnJobMemories",
                column: "UpdatedAt");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "SnJobMemories");
        }
    }
}
