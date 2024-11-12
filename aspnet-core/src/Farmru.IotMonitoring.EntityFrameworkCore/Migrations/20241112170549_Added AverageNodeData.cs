using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Farmru.IotMonitoring.Migrations
{
    /// <inheritdoc />
    public partial class AddedAverageNodeData : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "AverageNodeData",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    AvgSoilTemperature = table.Column<float>(type: "real", nullable: true),
                    AvgSoilPH = table.Column<float>(type: "real", nullable: true),
                    AvgMoisture = table.Column<float>(type: "real", nullable: true),
                    AvgPhosphorus = table.Column<float>(type: "real", nullable: true),
                    AvgPotassium = table.Column<float>(type: "real", nullable: true),
                    AvgNitrogen = table.Column<float>(type: "real", nullable: true),
                    AvgSolarPanelVoltage = table.Column<float>(type: "real", nullable: true),
                    AvgBatteryVoltage = table.Column<float>(type: "real", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_AverageNodeData", x => x.Id);
                });
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "AverageNodeData");
        }
    }
}
