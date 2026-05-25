using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Farmru.IotMonitoring.Migrations
{
    /// <inheritdoc />
    public partial class Added_Operational_Monitoring : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<DateTime>(
                name: "LastHealthEvaluatedAt",
                table: "Nodes",
                type: "datetime2",
                nullable: true);

            migrationBuilder.AddColumn<int>(
                name: "TelemetryQuality",
                table: "Nodes",
                type: "int",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.AddColumn<int>(
                name: "AnomalySensitivityPercent",
                table: "AlertThresholdConfigurations",
                type: "int",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.AddColumn<int>(
                name: "EscalationTimeoutMinutes",
                table: "AlertThresholdConfigurations",
                type: "int",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.AddColumn<bool>(
                name: "MonitoringEnabled",
                table: "AlertThresholdConfigurations",
                type: "bit",
                nullable: false,
                defaultValue: false);

            migrationBuilder.AddColumn<int>(
                name: "StaleTelemetryThresholdMinutes",
                table: "AlertThresholdConfigurations",
                type: "int",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.AddColumn<DateTime>(
                name: "EscalatedAt",
                table: "Alerts",
                type: "datetime2",
                nullable: true);

            migrationBuilder.AddColumn<int>(
                name: "EscalationLevel",
                table: "Alerts",
                type: "int",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.AddColumn<DateTime>(
                name: "LastEscalatedAt",
                table: "Alerts",
                type: "datetime2",
                nullable: true);

            migrationBuilder.CreateTable(
                name: "MonitoringExecutionHistories",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    TenantId = table.Column<int>(type: "int", nullable: true),
                    JobType = table.Column<int>(type: "int", nullable: false),
                    StartedAt = table.Column<DateTime>(type: "datetime2", nullable: false),
                    CompletedAt = table.Column<DateTime>(type: "datetime2", nullable: true),
                    DurationMs = table.Column<long>(type: "bigint", nullable: false),
                    Succeeded = table.Column<bool>(type: "bit", nullable: false),
                    ErrorMessage = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    AlertsGenerated = table.Column<int>(type: "int", nullable: false),
                    AlertsResolved = table.Column<int>(type: "int", nullable: false),
                    DevicesEvaluated = table.Column<int>(type: "int", nullable: false),
                    EscalationsPerformed = table.Column<int>(type: "int", nullable: false),
                    SummaryJson = table.Column<string>(type: "nvarchar(max)", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_MonitoringExecutionHistories", x => x.Id);
                });

            migrationBuilder.CreateIndex(
                name: "IX_MonitoringExecutionHistories_StartedAt",
                table: "MonitoringExecutionHistories",
                column: "StartedAt");

            migrationBuilder.CreateIndex(
                name: "IX_MonitoringExecutionHistories_TenantId_JobType_StartedAt",
                table: "MonitoringExecutionHistories",
                columns: new[] { "TenantId", "JobType", "StartedAt" });
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "MonitoringExecutionHistories");

            migrationBuilder.DropColumn(
                name: "LastHealthEvaluatedAt",
                table: "Nodes");

            migrationBuilder.DropColumn(
                name: "TelemetryQuality",
                table: "Nodes");

            migrationBuilder.DropColumn(
                name: "AnomalySensitivityPercent",
                table: "AlertThresholdConfigurations");

            migrationBuilder.DropColumn(
                name: "EscalationTimeoutMinutes",
                table: "AlertThresholdConfigurations");

            migrationBuilder.DropColumn(
                name: "MonitoringEnabled",
                table: "AlertThresholdConfigurations");

            migrationBuilder.DropColumn(
                name: "StaleTelemetryThresholdMinutes",
                table: "AlertThresholdConfigurations");

            migrationBuilder.DropColumn(
                name: "EscalatedAt",
                table: "Alerts");

            migrationBuilder.DropColumn(
                name: "EscalationLevel",
                table: "Alerts");

            migrationBuilder.DropColumn(
                name: "LastEscalatedAt",
                table: "Alerts");
        }
    }
}
