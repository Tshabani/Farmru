using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Farmru.IotMonitoring.Migrations
{
    /// <inheritdoc />
    public partial class RemovedUserfromFacapp : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_FacilityAppointments_AbpUsers_AppointedUserId",
                table: "FacilityAppointments");

            migrationBuilder.DropIndex(
                name: "IX_FacilityAppointments_AppointedUserId",
                table: "FacilityAppointments");

            migrationBuilder.DropColumn(
                name: "AppointedUserId",
                table: "FacilityAppointments");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<long>(
                name: "AppointedUserId",
                table: "FacilityAppointments",
                type: "bigint",
                nullable: true);

            migrationBuilder.CreateIndex(
                name: "IX_FacilityAppointments_AppointedUserId",
                table: "FacilityAppointments",
                column: "AppointedUserId");

            migrationBuilder.AddForeignKey(
                name: "FK_FacilityAppointments_AbpUsers_AppointedUserId",
                table: "FacilityAppointments",
                column: "AppointedUserId",
                principalTable: "AbpUsers",
                principalColumn: "Id");
        }
    }
}
