using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Farmru.IotMonitoring.Migrations
{
    /// <inheritdoc />
    public partial class Added_Incident_Workflow : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Incidents_People_AssignedToId",
                table: "Incidents");

            migrationBuilder.DropForeignKey(
                name: "FK_Incidents_People_CreatedById",
                table: "Incidents");

            migrationBuilder.AddColumn<DateTime>(
                name: "AcknowledgedAt",
                table: "Incidents",
                type: "datetime2",
                nullable: true);

            migrationBuilder.AddColumn<DateTime>(
                name: "AssignedAt",
                table: "Incidents",
                type: "datetime2",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "AssignedTeamName",
                table: "Incidents",
                type: "nvarchar(max)",
                nullable: true);

            migrationBuilder.AddColumn<DateTime>(
                name: "ClosedDate",
                table: "Incidents",
                type: "datetime2",
                nullable: true);

            migrationBuilder.AddColumn<int>(
                name: "EscalationLevel",
                table: "Incidents",
                type: "int",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.AddColumn<Guid>(
                name: "FacilityId",
                table: "Incidents",
                type: "uniqueidentifier",
                nullable: true);

            migrationBuilder.AddColumn<DateTime>(
                name: "FirstResponseAt",
                table: "Incidents",
                type: "datetime2",
                nullable: true);

            migrationBuilder.AddColumn<bool>(
                name: "IsEscalated",
                table: "Incidents",
                type: "bit",
                nullable: false,
                defaultValue: false);

            migrationBuilder.AddColumn<Guid>(
                name: "NodeId",
                table: "Incidents",
                type: "uniqueidentifier",
                nullable: true);

            migrationBuilder.AddColumn<Guid>(
                name: "RelatedAlertId",
                table: "Incidents",
                type: "uniqueidentifier",
                nullable: true);

            migrationBuilder.AddColumn<DateTime>(
                name: "ResolutionDueAt",
                table: "Incidents",
                type: "datetime2",
                nullable: false,
                defaultValue: new DateTime(1, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified));

            migrationBuilder.AddColumn<string>(
                name: "ResolutionNotes",
                table: "Incidents",
                type: "nvarchar(max)",
                nullable: true);

            migrationBuilder.AddColumn<DateTime>(
                name: "ResponseDueAt",
                table: "Incidents",
                type: "datetime2",
                nullable: false,
                defaultValue: new DateTime(1, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified));

            migrationBuilder.AddColumn<bool>(
                name: "SlaResolutionBreached",
                table: "Incidents",
                type: "bit",
                nullable: false,
                defaultValue: false);

            migrationBuilder.AddColumn<bool>(
                name: "SlaResponseBreached",
                table: "Incidents",
                type: "bit",
                nullable: false,
                defaultValue: false);

            migrationBuilder.AddColumn<int>(
                name: "SlaStatus",
                table: "Incidents",
                type: "int",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.AddColumn<int>(
                name: "TenantId",
                table: "Incidents",
                type: "int",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.CreateTable(
                name: "IncidentAssignments",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    TenantId = table.Column<int>(type: "int", nullable: false),
                    IncidentId = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    AssignedPersonId = table.Column<Guid>(type: "uniqueidentifier", nullable: true),
                    AssignedTeamName = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    AssignedByUserId = table.Column<long>(type: "bigint", nullable: true),
                    AssignedAt = table.Column<DateTime>(type: "datetime2", nullable: false),
                    UnassignedAt = table.Column<DateTime>(type: "datetime2", nullable: true),
                    DispatchNotes = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    IsActive = table.Column<bool>(type: "bit", nullable: false),
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
                    table.PrimaryKey("PK_IncidentAssignments", x => x.Id);
                    table.ForeignKey(
                        name: "FK_IncidentAssignments_Incidents_IncidentId",
                        column: x => x.IncidentId,
                        principalTable: "Incidents",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_IncidentAssignments_People_AssignedPersonId",
                        column: x => x.AssignedPersonId,
                        principalTable: "People",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateTable(
                name: "IncidentAttachments",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    TenantId = table.Column<int>(type: "int", nullable: false),
                    IncidentId = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    FileName = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    ContentType = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    StoragePath = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    FileSizeBytes = table.Column<long>(type: "bigint", nullable: false),
                    Caption = table.Column<string>(type: "nvarchar(max)", nullable: true),
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
                    table.PrimaryKey("PK_IncidentAttachments", x => x.Id);
                    table.ForeignKey(
                        name: "FK_IncidentAttachments_Incidents_IncidentId",
                        column: x => x.IncidentId,
                        principalTable: "Incidents",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "IncidentTimelineEvents",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    TenantId = table.Column<int>(type: "int", nullable: false),
                    IncidentId = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    EventType = table.Column<int>(type: "int", nullable: false),
                    Title = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    Description = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    MetadataJson = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    CreatorUserId = table.Column<long>(type: "bigint", nullable: true),
                    CreationTime = table.Column<DateTime>(type: "datetime2", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_IncidentTimelineEvents", x => x.Id);
                    table.ForeignKey(
                        name: "FK_IncidentTimelineEvents_Incidents_IncidentId",
                        column: x => x.IncidentId,
                        principalTable: "Incidents",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateIndex(
                name: "IX_Incidents_FacilityId",
                table: "Incidents",
                column: "FacilityId");

            migrationBuilder.CreateIndex(
                name: "IX_Incidents_TenantId_AssignedToId_Status",
                table: "Incidents",
                columns: new[] { "TenantId", "AssignedToId", "Status" });

            migrationBuilder.CreateIndex(
                name: "IX_Incidents_TenantId_SlaStatus_ResolutionDueAt",
                table: "Incidents",
                columns: new[] { "TenantId", "SlaStatus", "ResolutionDueAt" });

            migrationBuilder.CreateIndex(
                name: "IX_Incidents_TenantId_Status_Priority",
                table: "Incidents",
                columns: new[] { "TenantId", "Status", "Priority" });

            migrationBuilder.CreateIndex(
                name: "IX_IncidentAssignments_AssignedPersonId",
                table: "IncidentAssignments",
                column: "AssignedPersonId");

            migrationBuilder.CreateIndex(
                name: "IX_IncidentAssignments_IncidentId",
                table: "IncidentAssignments",
                column: "IncidentId");

            migrationBuilder.CreateIndex(
                name: "IX_IncidentAssignments_TenantId_AssignedPersonId_IsActive",
                table: "IncidentAssignments",
                columns: new[] { "TenantId", "AssignedPersonId", "IsActive" });

            migrationBuilder.CreateIndex(
                name: "IX_IncidentAssignments_TenantId_IncidentId_IsActive",
                table: "IncidentAssignments",
                columns: new[] { "TenantId", "IncidentId", "IsActive" });

            migrationBuilder.CreateIndex(
                name: "IX_IncidentAttachments_IncidentId",
                table: "IncidentAttachments",
                column: "IncidentId");

            migrationBuilder.CreateIndex(
                name: "IX_IncidentAttachments_TenantId_IncidentId",
                table: "IncidentAttachments",
                columns: new[] { "TenantId", "IncidentId" });

            migrationBuilder.CreateIndex(
                name: "IX_IncidentTimelineEvents_IncidentId",
                table: "IncidentTimelineEvents",
                column: "IncidentId");

            migrationBuilder.CreateIndex(
                name: "IX_IncidentTimelineEvents_TenantId_IncidentId_CreationTime",
                table: "IncidentTimelineEvents",
                columns: new[] { "TenantId", "IncidentId", "CreationTime" });

            migrationBuilder.AddForeignKey(
                name: "FK_Incidents_Facilities_FacilityId",
                table: "Incidents",
                column: "FacilityId",
                principalTable: "Facilities",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);

            migrationBuilder.AddForeignKey(
                name: "FK_Incidents_People_AssignedToId",
                table: "Incidents",
                column: "AssignedToId",
                principalTable: "People",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);

            migrationBuilder.AddForeignKey(
                name: "FK_Incidents_People_CreatedById",
                table: "Incidents",
                column: "CreatedById",
                principalTable: "People",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);

            migrationBuilder.Sql(@"
UPDATE Incidents SET Status = CASE Status
    WHEN 1 THEN 2
    WHEN 2 THEN 5
    WHEN 3 THEN 6
    ELSE Status
END
WHERE Status IN (1, 2, 3);
");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Incidents_Facilities_FacilityId",
                table: "Incidents");

            migrationBuilder.DropForeignKey(
                name: "FK_Incidents_People_AssignedToId",
                table: "Incidents");

            migrationBuilder.DropForeignKey(
                name: "FK_Incidents_People_CreatedById",
                table: "Incidents");

            migrationBuilder.DropTable(
                name: "IncidentAssignments");

            migrationBuilder.DropTable(
                name: "IncidentAttachments");

            migrationBuilder.DropTable(
                name: "IncidentTimelineEvents");

            migrationBuilder.DropIndex(
                name: "IX_Incidents_FacilityId",
                table: "Incidents");

            migrationBuilder.DropIndex(
                name: "IX_Incidents_TenantId_AssignedToId_Status",
                table: "Incidents");

            migrationBuilder.DropIndex(
                name: "IX_Incidents_TenantId_SlaStatus_ResolutionDueAt",
                table: "Incidents");

            migrationBuilder.DropIndex(
                name: "IX_Incidents_TenantId_Status_Priority",
                table: "Incidents");

            migrationBuilder.DropColumn(
                name: "AcknowledgedAt",
                table: "Incidents");

            migrationBuilder.DropColumn(
                name: "AssignedAt",
                table: "Incidents");

            migrationBuilder.DropColumn(
                name: "AssignedTeamName",
                table: "Incidents");

            migrationBuilder.DropColumn(
                name: "ClosedDate",
                table: "Incidents");

            migrationBuilder.DropColumn(
                name: "EscalationLevel",
                table: "Incidents");

            migrationBuilder.DropColumn(
                name: "FacilityId",
                table: "Incidents");

            migrationBuilder.DropColumn(
                name: "FirstResponseAt",
                table: "Incidents");

            migrationBuilder.DropColumn(
                name: "IsEscalated",
                table: "Incidents");

            migrationBuilder.DropColumn(
                name: "NodeId",
                table: "Incidents");

            migrationBuilder.DropColumn(
                name: "RelatedAlertId",
                table: "Incidents");

            migrationBuilder.DropColumn(
                name: "ResolutionDueAt",
                table: "Incidents");

            migrationBuilder.DropColumn(
                name: "ResolutionNotes",
                table: "Incidents");

            migrationBuilder.DropColumn(
                name: "ResponseDueAt",
                table: "Incidents");

            migrationBuilder.DropColumn(
                name: "SlaResolutionBreached",
                table: "Incidents");

            migrationBuilder.DropColumn(
                name: "SlaResponseBreached",
                table: "Incidents");

            migrationBuilder.DropColumn(
                name: "SlaStatus",
                table: "Incidents");

            migrationBuilder.DropColumn(
                name: "TenantId",
                table: "Incidents");

            migrationBuilder.AddForeignKey(
                name: "FK_Incidents_People_AssignedToId",
                table: "Incidents",
                column: "AssignedToId",
                principalTable: "People",
                principalColumn: "Id");

            migrationBuilder.AddForeignKey(
                name: "FK_Incidents_People_CreatedById",
                table: "Incidents",
                column: "CreatedById",
                principalTable: "People",
                principalColumn: "Id");
        }
    }
}
