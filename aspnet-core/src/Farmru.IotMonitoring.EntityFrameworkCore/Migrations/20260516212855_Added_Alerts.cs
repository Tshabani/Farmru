using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Farmru.IotMonitoring.Migrations
{
    /// <inheritdoc />
    public partial class Added_Alerts : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "FullName",
                table: "People");

            migrationBuilder.AlterColumn<int>(
                name: "Status",
                table: "Incidents",
                type: "int",
                nullable: false,
                defaultValue: 0,
                oldClrType: typeof(int),
                oldType: "int",
                oldNullable: true);

            migrationBuilder.AlterColumn<int>(
                name: "Priority",
                table: "Incidents",
                type: "int",
                nullable: false,
                defaultValue: 0,
                oldClrType: typeof(int),
                oldType: "int",
                oldNullable: true);

            migrationBuilder.CreateTable(
                name: "Alerts",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    TenantId = table.Column<int>(type: "int", nullable: false),
                    NodeId = table.Column<Guid>(type: "uniqueidentifier", nullable: true),
                    FacilityId = table.Column<Guid>(type: "uniqueidentifier", nullable: true),
                    AlertType = table.Column<int>(type: "int", nullable: false),
                    Severity = table.Column<int>(type: "int", nullable: false),
                    Title = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    Description = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    IsAcknowledged = table.Column<bool>(type: "bit", nullable: false),
                    AcknowledgedByUserId = table.Column<long>(type: "bigint", nullable: true),
                    AcknowledgedAt = table.Column<DateTime>(type: "datetime2", nullable: true),
                    IsResolved = table.Column<bool>(type: "bit", nullable: false),
                    ResolvedByUserId = table.Column<long>(type: "bigint", nullable: true),
                    ResolvedAt = table.Column<DateTime>(type: "datetime2", nullable: true),
                    ResolutionNotes = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    TriggeredAt = table.Column<DateTime>(type: "datetime2", nullable: false),
                    LastTriggeredAt = table.Column<DateTime>(type: "datetime2", nullable: true),
                    IsActive = table.Column<bool>(type: "bit", nullable: false),
                    MetadataJson = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    SourceTelemetryId = table.Column<Guid>(type: "uniqueidentifier", nullable: true),
                    CreationTime = table.Column<DateTime>(type: "datetime2", nullable: false),
                    CreatorUserId = table.Column<long>(type: "bigint", nullable: true),
                    LastModificationTime = table.Column<DateTime>(type: "datetime2", nullable: true),
                    LastModifierUserId = table.Column<long>(type: "bigint", nullable: true),
                    IsDeleted = table.Column<bool>(type: "bit", nullable: false),
                    DeleterUserId = table.Column<long>(type: "bigint", nullable: true),
                    DeletionTime = table.Column<DateTime>(type: "datetime2", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Alerts", x => x.Id);
                    table.ForeignKey(
                        name: "FK_Alerts_Facilities_FacilityId",
                        column: x => x.FacilityId,
                        principalTable: "Facilities",
                        principalColumn: "Id");
                    table.ForeignKey(
                        name: "FK_Alerts_Nodes_NodeId",
                        column: x => x.NodeId,
                        principalTable: "Nodes",
                        principalColumn: "Id");
                });

            migrationBuilder.CreateTable(
                name: "AlertThresholdConfigurations",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    TenantId = table.Column<int>(type: "int", nullable: false),
                    FacilityId = table.Column<Guid>(type: "uniqueidentifier", nullable: true),
                    MinimumBatteryPercent = table.Column<decimal>(type: "decimal(18,2)", nullable: false),
                    CriticalBatteryPercent = table.Column<decimal>(type: "decimal(18,2)", nullable: false),
                    MaximumTemperature = table.Column<decimal>(type: "decimal(18,2)", nullable: false),
                    MinimumTemperature = table.Column<decimal>(type: "decimal(18,2)", nullable: false),
                    MinimumMoisturePercent = table.Column<decimal>(type: "decimal(18,2)", nullable: false),
                    OfflineTimeoutMinutes = table.Column<int>(type: "int", nullable: false),
                    AutoResolveWhenNormalized = table.Column<bool>(type: "bit", nullable: false),
                    CreationTime = table.Column<DateTime>(type: "datetime2", nullable: false),
                    CreatorUserId = table.Column<long>(type: "bigint", nullable: true),
                    LastModificationTime = table.Column<DateTime>(type: "datetime2", nullable: true),
                    LastModifierUserId = table.Column<long>(type: "bigint", nullable: true),
                    IsDeleted = table.Column<bool>(type: "bit", nullable: false),
                    DeleterUserId = table.Column<long>(type: "bigint", nullable: true),
                    DeletionTime = table.Column<DateTime>(type: "datetime2", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_AlertThresholdConfigurations", x => x.Id);
                    table.ForeignKey(
                        name: "FK_AlertThresholdConfigurations_Facilities_FacilityId",
                        column: x => x.FacilityId,
                        principalTable: "Facilities",
                        principalColumn: "Id");
                });

            migrationBuilder.CreateIndex(
                name: "IX_Alerts_FacilityId",
                table: "Alerts",
                column: "FacilityId");

            migrationBuilder.CreateIndex(
                name: "IX_Alerts_NodeId",
                table: "Alerts",
                column: "NodeId");

            migrationBuilder.CreateIndex(
                name: "IX_Alerts_TenantId_IsActive_IsResolved",
                table: "Alerts",
                columns: new[] { "TenantId", "IsActive", "IsResolved" });

            migrationBuilder.CreateIndex(
                name: "IX_Alerts_TenantId_NodeId_AlertType_IsActive",
                table: "Alerts",
                columns: new[] { "TenantId", "NodeId", "AlertType", "IsActive" });

            migrationBuilder.CreateIndex(
                name: "IX_Alerts_TenantId_Severity_TriggeredAt",
                table: "Alerts",
                columns: new[] { "TenantId", "Severity", "TriggeredAt" });

            migrationBuilder.CreateIndex(
                name: "IX_Alerts_TriggeredAt",
                table: "Alerts",
                column: "TriggeredAt");

            migrationBuilder.CreateIndex(
                name: "IX_AlertThresholdConfigurations_FacilityId",
                table: "AlertThresholdConfigurations",
                column: "FacilityId");

            migrationBuilder.CreateIndex(
                name: "IX_AlertThresholdConfigurations_TenantId_FacilityId",
                table: "AlertThresholdConfigurations",
                columns: new[] { "TenantId", "FacilityId" });
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "Alerts");

            migrationBuilder.DropTable(
                name: "AlertThresholdConfigurations");

            migrationBuilder.AddColumn<string>(
                name: "FullName",
                table: "People",
                type: "nvarchar(max)",
                nullable: true);

            migrationBuilder.AlterColumn<int>(
                name: "Status",
                table: "Incidents",
                type: "int",
                nullable: true,
                oldClrType: typeof(int),
                oldType: "int");

            migrationBuilder.AlterColumn<int>(
                name: "Priority",
                table: "Incidents",
                type: "int",
                nullable: true,
                oldClrType: typeof(int),
                oldType: "int");
        }
    }
}
