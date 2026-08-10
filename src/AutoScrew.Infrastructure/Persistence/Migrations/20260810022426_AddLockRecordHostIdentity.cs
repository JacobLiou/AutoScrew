using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace AutoScrew.Infrastructure.Persistence.Migrations
{
    /// <inheritdoc />
    public partial class AddLockRecordHostIdentity : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<string>(
                name: "HostIp",
                table: "LockRecords",
                type: "TEXT",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "HostMac",
                table: "LockRecords",
                type: "TEXT",
                nullable: true);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "HostIp",
                table: "LockRecords");

            migrationBuilder.DropColumn(
                name: "HostMac",
                table: "LockRecords");
        }
    }
}
