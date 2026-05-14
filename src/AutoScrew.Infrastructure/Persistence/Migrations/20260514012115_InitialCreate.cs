using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace AutoScrew.Infrastructure.Persistence.Migrations
{
    /// <inheritdoc />
    public partial class InitialCreate : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "ErrorLogs",
                columns: table => new
                {
                    Id = table.Column<long>(type: "INTEGER", nullable: false)
                        .Annotation("Sqlite:Autoincrement", true),
                    LockRecordId = table.Column<long>(type: "INTEGER", nullable: true),
                    ErrorCode = table.Column<string>(type: "TEXT", nullable: false),
                    Message = table.Column<string>(type: "TEXT", nullable: false),
                    ResolveBy = table.Column<string>(type: "TEXT", nullable: true),
                    CreatedAt = table.Column<DateTimeOffset>(type: "TEXT", nullable: false),
                    ResolveTime = table.Column<DateTimeOffset>(type: "TEXT", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_ErrorLogs", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "LockRecords",
                columns: table => new
                {
                    Id = table.Column<long>(type: "INTEGER", nullable: false)
                        .Annotation("Sqlite:Autoincrement", true),
                    SerialNumber = table.Column<string>(type: "TEXT", nullable: false),
                    PartNumber = table.Column<string>(type: "TEXT", nullable: false),
                    StationId = table.Column<string>(type: "TEXT", nullable: false),
                    OperatorId = table.Column<string>(type: "TEXT", nullable: false),
                    StartedAt = table.Column<DateTimeOffset>(type: "TEXT", nullable: false),
                    EndedAt = table.Column<DateTimeOffset>(type: "TEXT", nullable: true),
                    Result = table.Column<string>(type: "TEXT", nullable: false),
                    IsRework = table.Column<bool>(type: "INTEGER", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_LockRecords", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "OutboxUploads",
                columns: table => new
                {
                    Id = table.Column<long>(type: "INTEGER", nullable: false)
                        .Annotation("Sqlite:Autoincrement", true),
                    IdempotencyKey = table.Column<string>(type: "TEXT", nullable: false),
                    PayloadJson = table.Column<string>(type: "TEXT", nullable: false),
                    CreatedAt = table.Column<DateTimeOffset>(type: "TEXT", nullable: false),
                    SentAt = table.Column<DateTimeOffset>(type: "TEXT", nullable: true),
                    RetryCount = table.Column<int>(type: "INTEGER", nullable: false),
                    LastError = table.Column<string>(type: "TEXT", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_OutboxUploads", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "ScrewDetails",
                columns: table => new
                {
                    Id = table.Column<long>(type: "INTEGER", nullable: false)
                        .Annotation("Sqlite:Autoincrement", true),
                    LockRecordId = table.Column<long>(type: "INTEGER", nullable: false),
                    PositionIndex = table.Column<int>(type: "INTEGER", nullable: false),
                    PartNo = table.Column<string>(type: "TEXT", nullable: true),
                    FinalTorqueNm = table.Column<double>(type: "REAL", nullable: true),
                    FinalAngleDeg = table.Column<double>(type: "REAL", nullable: true),
                    CurvePath = table.Column<string>(type: "TEXT", nullable: true),
                    Result = table.Column<string>(type: "TEXT", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_ScrewDetails", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "SessionCheckpoints",
                columns: table => new
                {
                    Id = table.Column<int>(type: "INTEGER", nullable: false)
                        .Annotation("Sqlite:Autoincrement", true),
                    PayloadJson = table.Column<string>(type: "TEXT", nullable: false),
                    UpdatedAt = table.Column<DateTimeOffset>(type: "TEXT", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_SessionCheckpoints", x => x.Id);
                });

            migrationBuilder.CreateIndex(
                name: "IX_ErrorLogs_LockRecordId",
                table: "ErrorLogs",
                column: "LockRecordId");

            migrationBuilder.CreateIndex(
                name: "IX_LockRecords_SerialNumber",
                table: "LockRecords",
                column: "SerialNumber");

            migrationBuilder.CreateIndex(
                name: "IX_LockRecords_StartedAt",
                table: "LockRecords",
                column: "StartedAt");

            migrationBuilder.CreateIndex(
                name: "IX_OutboxUploads_IdempotencyKey",
                table: "OutboxUploads",
                column: "IdempotencyKey");

            migrationBuilder.CreateIndex(
                name: "IX_OutboxUploads_SentAt",
                table: "OutboxUploads",
                column: "SentAt");

            migrationBuilder.CreateIndex(
                name: "IX_ScrewDetails_LockRecordId",
                table: "ScrewDetails",
                column: "LockRecordId");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "ErrorLogs");

            migrationBuilder.DropTable(
                name: "LockRecords");

            migrationBuilder.DropTable(
                name: "OutboxUploads");

            migrationBuilder.DropTable(
                name: "ScrewDetails");

            migrationBuilder.DropTable(
                name: "SessionCheckpoints");
        }
    }
}
