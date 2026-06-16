using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace AutoScrew.Infrastructure.Persistence.Migrations
{
    /// <inheritdoc />
    public partial class AddProductTemplateSync : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "ProductTemplateSyncs",
                columns: table => new
                {
                    PartNumber = table.Column<string>(type: "TEXT", nullable: false),
                    LocalRelativePath = table.Column<string>(type: "TEXT", nullable: false),
                    SyncState = table.Column<int>(type: "INTEGER", nullable: false),
                    LocalFileHash = table.Column<string>(type: "TEXT", nullable: true),
                    LocalModifiedUtc = table.Column<DateTimeOffset>(type: "TEXT", nullable: true),
                    LastMesPullUtc = table.Column<DateTimeOffset>(type: "TEXT", nullable: true),
                    LastMesPushUtc = table.Column<DateTimeOffset>(type: "TEXT", nullable: true),
                    MesRevision = table.Column<string>(type: "TEXT", nullable: true),
                    LastError = table.Column<string>(type: "TEXT", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_ProductTemplateSyncs", x => x.PartNumber);
                });

            migrationBuilder.CreateIndex(
                name: "IX_ProductTemplateSyncs_SyncState",
                table: "ProductTemplateSyncs",
                column: "SyncState");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "ProductTemplateSyncs");
        }
    }
}
