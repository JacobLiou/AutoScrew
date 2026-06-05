using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace AutoScrew.Infrastructure.Persistence.Migrations
{
    /// <inheritdoc />
    public partial class AddUserAuditLog : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "UserAuditLogs",
                columns: table => new
                {
                    Id = table.Column<long>(type: "INTEGER", nullable: false)
                        .Annotation("Sqlite:Autoincrement", true),
                    Timestamp = table.Column<DateTimeOffset>(type: "TEXT", nullable: false),
                    StationId = table.Column<string>(type: "TEXT", nullable: false),
                    UserId = table.Column<string>(type: "TEXT", nullable: false),
                    DisplayName = table.Column<string>(type: "TEXT", nullable: false),
                    Role = table.Column<int>(type: "INTEGER", nullable: false),
                    Category = table.Column<int>(type: "INTEGER", nullable: false),
                    Action = table.Column<string>(type: "TEXT", nullable: false),
                    Target = table.Column<string>(type: "TEXT", nullable: true),
                    Detail = table.Column<string>(type: "TEXT", nullable: true),
                    Success = table.Column<bool>(type: "INTEGER", nullable: false),
                    SerialNumber = table.Column<string>(type: "TEXT", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_UserAuditLogs", x => x.Id);
                });

            migrationBuilder.CreateIndex(
                name: "IX_UserAuditLogs_Action",
                table: "UserAuditLogs",
                column: "Action");

            migrationBuilder.CreateIndex(
                name: "IX_UserAuditLogs_Category",
                table: "UserAuditLogs",
                column: "Category");

            migrationBuilder.CreateIndex(
                name: "IX_UserAuditLogs_Timestamp",
                table: "UserAuditLogs",
                column: "Timestamp");

            migrationBuilder.CreateIndex(
                name: "IX_UserAuditLogs_UserId",
                table: "UserAuditLogs",
                column: "UserId");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "UserAuditLogs");
        }
    }
}
