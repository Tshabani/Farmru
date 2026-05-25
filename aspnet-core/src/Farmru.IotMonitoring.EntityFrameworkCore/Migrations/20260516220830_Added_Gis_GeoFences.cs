using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Farmru.IotMonitoring.Migrations
{
    /// <inheritdoc />
    public partial class Added_Gis_GeoFences : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<decimal>(
                name: "LastKnownLatitude",
                table: "Nodes",
                type: "decimal(18,2)",
                nullable: true);

            migrationBuilder.AddColumn<decimal>(
                name: "LastKnownLongitude",
                table: "Nodes",
                type: "decimal(18,2)",
                nullable: true);

            migrationBuilder.AddColumn<decimal>(
                name: "Latitude",
                table: "Incidents",
                type: "decimal(18,2)",
                nullable: true);

            migrationBuilder.AddColumn<decimal>(
                name: "Longitude",
                table: "Incidents",
                type: "decimal(18,2)",
                nullable: true);

            migrationBuilder.CreateTable(
                name: "GeoFences",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    TenantId = table.Column<int>(type: "int", nullable: false),
                    Name = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    Description = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    GeoFenceType = table.Column<int>(type: "int", nullable: false),
                    CenterLatitude = table.Column<decimal>(type: "decimal(18,2)", nullable: true),
                    CenterLongitude = table.Column<decimal>(type: "decimal(18,2)", nullable: true),
                    RadiusMeters = table.Column<double>(type: "float", nullable: true),
                    PolygonJson = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    IsActive = table.Column<bool>(type: "bit", nullable: false),
                    Severity = table.Column<int>(type: "int", nullable: false),
                    FacilityId = table.Column<Guid>(type: "uniqueidentifier", nullable: true),
                    TriggerAlertOnExit = table.Column<bool>(type: "bit", nullable: false),
                    TriggerAlertOnEntry = table.Column<bool>(type: "bit", nullable: false),
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
                    table.PrimaryKey("PK_GeoFences", x => x.Id);
                    table.ForeignKey(
                        name: "FK_GeoFences_Facilities_FacilityId",
                        column: x => x.FacilityId,
                        principalTable: "Facilities",
                        principalColumn: "Id");
                });

            migrationBuilder.CreateIndex(
                name: "IX_Nodes_TenantId_LastKnownLatitude_LastKnownLongitude",
                table: "Nodes",
                columns: new[] { "TenantId", "LastKnownLatitude", "LastKnownLongitude" });

            migrationBuilder.CreateIndex(
                name: "IX_Incidents_Latitude_Longitude",
                table: "Incidents",
                columns: new[] { "Latitude", "Longitude" });

            migrationBuilder.CreateIndex(
                name: "IX_GeoFences_FacilityId",
                table: "GeoFences",
                column: "FacilityId");

            migrationBuilder.CreateIndex(
                name: "IX_GeoFences_TenantId_FacilityId",
                table: "GeoFences",
                columns: new[] { "TenantId", "FacilityId" });

            migrationBuilder.CreateIndex(
                name: "IX_GeoFences_TenantId_IsActive",
                table: "GeoFences",
                columns: new[] { "TenantId", "IsActive" });
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "GeoFences");

            migrationBuilder.DropIndex(
                name: "IX_Nodes_TenantId_LastKnownLatitude_LastKnownLongitude",
                table: "Nodes");

            migrationBuilder.DropIndex(
                name: "IX_Incidents_Latitude_Longitude",
                table: "Incidents");

            migrationBuilder.DropColumn(
                name: "LastKnownLatitude",
                table: "Nodes");

            migrationBuilder.DropColumn(
                name: "LastKnownLongitude",
                table: "Nodes");

            migrationBuilder.DropColumn(
                name: "Latitude",
                table: "Incidents");

            migrationBuilder.DropColumn(
                name: "Longitude",
                table: "Incidents");
        }
    }
}
