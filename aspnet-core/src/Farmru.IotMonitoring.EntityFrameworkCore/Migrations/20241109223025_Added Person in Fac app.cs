using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Farmru.IotMonitoring.Migrations
{
    /// <inheritdoc />
    public partial class AddedPersoninFacapp : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<Guid>(
                name: "AppointedUserId",
                table: "FacilityAppointments",
                type: "uniqueidentifier",
                nullable: true);

            migrationBuilder.CreateIndex(
                name: "IX_FacilityAppointments_AppointedUserId",
                table: "FacilityAppointments",
                column: "AppointedUserId");

            migrationBuilder.AddForeignKey(
                name: "FK_FacilityAppointments_Peaople_AppointedUserId",
                table: "FacilityAppointments",
                column: "AppointedUserId",
                principalTable: "Peaople",
                principalColumn: "Id");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_FacilityAppointments_Peaople_AppointedUserId",
                table: "FacilityAppointments");

            migrationBuilder.DropIndex(
                name: "IX_FacilityAppointments_AppointedUserId",
                table: "FacilityAppointments");

            migrationBuilder.DropColumn(
                name: "AppointedUserId",
                table: "FacilityAppointments");
        }
    }
}
