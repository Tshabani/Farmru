using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Farmru.IotMonitoring.Migrations
{
    /// <inheritdoc />
    public partial class Added_Device_Lifecycle : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<decimal>(
                name: "BatteryLevel",
                table: "Nodes",
                type: "decimal(18,2)",
                nullable: true);

            migrationBuilder.AddColumn<int>(
                name: "DeviceStatus",
                table: "Nodes",
                type: "int",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.AddColumn<string>(
                name: "DisplayName",
                table: "Nodes",
                type: "nvarchar(max)",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "FirmwareVersion",
                table: "Nodes",
                type: "nvarchar(max)",
                nullable: true);

            migrationBuilder.AddColumn<int>(
                name: "HealthStatus",
                table: "Nodes",
                type: "int",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.AddColumn<bool>(
                name: "IsActive",
                table: "Nodes",
                type: "bit",
                nullable: false,
                defaultValue: false);

            migrationBuilder.AddColumn<DateTime>(
                name: "LastSeenAt",
                table: "Nodes",
                type: "datetime2",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "Notes",
                table: "Nodes",
                type: "nvarchar(max)",
                nullable: true);

            migrationBuilder.AddColumn<int>(
                name: "SignalStrength",
                table: "Nodes",
                type: "int",
                nullable: true);

            migrationBuilder.CreateTable(
                name: "NodeReplacementHistories",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    TenantId = table.Column<int>(type: "int", nullable: false),
                    NodeId = table.Column<Guid>(type: "uniqueidentifier", nullable: true),
                    OldSerialNumber = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    NewSerialNumber = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    ReplacedAt = table.Column<DateTime>(type: "datetime2", nullable: false),
                    Reason = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    Notes = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    CreationTime = table.Column<DateTime>(type: "datetime2", nullable: false),
                    CreatorUserId = table.Column<long>(type: "bigint", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_NodeReplacementHistories", x => x.Id);
                    table.ForeignKey(
                        name: "FK_NodeReplacementHistories_Nodes_NodeId",
                        column: x => x.NodeId,
                        principalTable: "Nodes",
                        principalColumn: "Id");
                });

            migrationBuilder.CreateIndex(
                name: "IX_NodeReplacementHistories_NodeId",
                table: "NodeReplacementHistories",
                column: "NodeId");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "NodeReplacementHistories");

            migrationBuilder.DropColumn(
                name: "BatteryLevel",
                table: "Nodes");

            migrationBuilder.DropColumn(
                name: "DeviceStatus",
                table: "Nodes");

            migrationBuilder.DropColumn(
                name: "DisplayName",
                table: "Nodes");

            migrationBuilder.DropColumn(
                name: "FirmwareVersion",
                table: "Nodes");

            migrationBuilder.DropColumn(
                name: "HealthStatus",
                table: "Nodes");

            migrationBuilder.DropColumn(
                name: "IsActive",
                table: "Nodes");

            migrationBuilder.DropColumn(
                name: "LastSeenAt",
                table: "Nodes");

            migrationBuilder.DropColumn(
                name: "Notes",
                table: "Nodes");

            migrationBuilder.DropColumn(
                name: "SignalStrength",
                table: "Nodes");
        }
    }
}
