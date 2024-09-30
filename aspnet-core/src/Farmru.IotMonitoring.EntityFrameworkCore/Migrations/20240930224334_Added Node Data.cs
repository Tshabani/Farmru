using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Farmru.IotMonitoring.Migrations
{
    /// <inheritdoc />
    public partial class AddedNodeData : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "NodeDatas",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    SoilTemperature = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    SoilPH = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    Moisture = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    Phosphorus = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    Potassium = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    Nitrogen = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    Latitude = table.Column<long>(type: "bigint", nullable: true),
                    Longitude = table.Column<long>(type: "bigint", nullable: true),
                    SolarPanelVoltage = table.Column<long>(type: "bigint", nullable: true),
                    BatteryVoltage = table.Column<long>(type: "bigint", nullable: true),
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
                    table.PrimaryKey("PK_NodeDatas", x => x.Id);
                });
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "NodeDatas");
        }
    }
}
