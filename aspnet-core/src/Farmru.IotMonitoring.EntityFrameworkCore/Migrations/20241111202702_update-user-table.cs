using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Farmru.IotMonitoring.Migrations
{
    /// <inheritdoc />
    public partial class updateusertable : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Facilities_Peaople_PrimaryContactId",
                table: "Facilities");

            migrationBuilder.DropForeignKey(
                name: "FK_FacilityAppointments_Peaople_AppointedUserId",
                table: "FacilityAppointments");

            migrationBuilder.DropForeignKey(
                name: "FK_Incidents_Peaople_AssignedToId",
                table: "Incidents");

            migrationBuilder.DropForeignKey(
                name: "FK_Incidents_Peaople_CreatedById",
                table: "Incidents");

            migrationBuilder.DropForeignKey(
                name: "FK_Peaople_AbpUsers_UserId",
                table: "Peaople");

            migrationBuilder.DropForeignKey(
                name: "FK_Tasks_Peaople_AssignedById",
                table: "Tasks");

            migrationBuilder.DropForeignKey(
                name: "FK_Tasks_Peaople_AssignedToId",
                table: "Tasks");

            migrationBuilder.DropPrimaryKey(
                name: "PK_Peaople",
                table: "Peaople");

            migrationBuilder.DropColumn(
                name: "Name",
                table: "AbpUsers");

            migrationBuilder.DropColumn(
                name: "Surname",
                table: "AbpUsers");

            migrationBuilder.DropColumn(
                name: "MiddleName",
                table: "Peaople");

            migrationBuilder.RenameTable(
                name: "Peaople",
                newName: "People");

            migrationBuilder.RenameColumn(
                name: "MobileNumber2",
                table: "People",
                newName: "MobileNumber");

            migrationBuilder.RenameColumn(
                name: "MobileNumber1",
                table: "People",
                newName: "AltMobileNumber");

            migrationBuilder.RenameColumn(
                name: "EmailAddress2",
                table: "People",
                newName: "EmailAddress");

            migrationBuilder.RenameColumn(
                name: "EmailAddress1",
                table: "People",
                newName: "AltEmailAddress");

            migrationBuilder.RenameIndex(
                name: "IX_Peaople_UserId",
                table: "People",
                newName: "IX_People_UserId");

            migrationBuilder.AddPrimaryKey(
                name: "PK_People",
                table: "People",
                column: "Id");

            migrationBuilder.AddForeignKey(
                name: "FK_Facilities_People_PrimaryContactId",
                table: "Facilities",
                column: "PrimaryContactId",
                principalTable: "People",
                principalColumn: "Id");

            migrationBuilder.AddForeignKey(
                name: "FK_FacilityAppointments_People_AppointedUserId",
                table: "FacilityAppointments",
                column: "AppointedUserId",
                principalTable: "People",
                principalColumn: "Id");

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

            migrationBuilder.AddForeignKey(
                name: "FK_People_AbpUsers_UserId",
                table: "People",
                column: "UserId",
                principalTable: "AbpUsers",
                principalColumn: "Id");

            migrationBuilder.AddForeignKey(
                name: "FK_Tasks_People_AssignedById",
                table: "Tasks",
                column: "AssignedById",
                principalTable: "People",
                principalColumn: "Id");

            migrationBuilder.AddForeignKey(
                name: "FK_Tasks_People_AssignedToId",
                table: "Tasks",
                column: "AssignedToId",
                principalTable: "People",
                principalColumn: "Id");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Facilities_People_PrimaryContactId",
                table: "Facilities");

            migrationBuilder.DropForeignKey(
                name: "FK_FacilityAppointments_People_AppointedUserId",
                table: "FacilityAppointments");

            migrationBuilder.DropForeignKey(
                name: "FK_Incidents_People_AssignedToId",
                table: "Incidents");

            migrationBuilder.DropForeignKey(
                name: "FK_Incidents_People_CreatedById",
                table: "Incidents");

            migrationBuilder.DropForeignKey(
                name: "FK_People_AbpUsers_UserId",
                table: "People");

            migrationBuilder.DropForeignKey(
                name: "FK_Tasks_People_AssignedById",
                table: "Tasks");

            migrationBuilder.DropForeignKey(
                name: "FK_Tasks_People_AssignedToId",
                table: "Tasks");

            migrationBuilder.DropPrimaryKey(
                name: "PK_People",
                table: "People");

            migrationBuilder.RenameTable(
                name: "People",
                newName: "Peaople");

            migrationBuilder.RenameColumn(
                name: "MobileNumber",
                table: "Peaople",
                newName: "MobileNumber2");

            migrationBuilder.RenameColumn(
                name: "EmailAddress",
                table: "Peaople",
                newName: "EmailAddress2");

            migrationBuilder.RenameColumn(
                name: "AltMobileNumber",
                table: "Peaople",
                newName: "MobileNumber1");

            migrationBuilder.RenameColumn(
                name: "AltEmailAddress",
                table: "Peaople",
                newName: "EmailAddress1");

            migrationBuilder.RenameIndex(
                name: "IX_People_UserId",
                table: "Peaople",
                newName: "IX_Peaople_UserId");

            migrationBuilder.AddColumn<string>(
                name: "Name",
                table: "AbpUsers",
                type: "nvarchar(64)",
                maxLength: 64,
                nullable: false,
                defaultValue: "");

            migrationBuilder.AddColumn<string>(
                name: "Surname",
                table: "AbpUsers",
                type: "nvarchar(64)",
                maxLength: 64,
                nullable: false,
                defaultValue: "");

            migrationBuilder.AddColumn<string>(
                name: "MiddleName",
                table: "Peaople",
                type: "nvarchar(50)",
                maxLength: 50,
                nullable: true);

            migrationBuilder.AddPrimaryKey(
                name: "PK_Peaople",
                table: "Peaople",
                column: "Id");

            migrationBuilder.AddForeignKey(
                name: "FK_Facilities_Peaople_PrimaryContactId",
                table: "Facilities",
                column: "PrimaryContactId",
                principalTable: "Peaople",
                principalColumn: "Id");

            migrationBuilder.AddForeignKey(
                name: "FK_FacilityAppointments_Peaople_AppointedUserId",
                table: "FacilityAppointments",
                column: "AppointedUserId",
                principalTable: "Peaople",
                principalColumn: "Id");

            migrationBuilder.AddForeignKey(
                name: "FK_Incidents_Peaople_AssignedToId",
                table: "Incidents",
                column: "AssignedToId",
                principalTable: "Peaople",
                principalColumn: "Id");

            migrationBuilder.AddForeignKey(
                name: "FK_Incidents_Peaople_CreatedById",
                table: "Incidents",
                column: "CreatedById",
                principalTable: "Peaople",
                principalColumn: "Id");

            migrationBuilder.AddForeignKey(
                name: "FK_Peaople_AbpUsers_UserId",
                table: "Peaople",
                column: "UserId",
                principalTable: "AbpUsers",
                principalColumn: "Id");

            migrationBuilder.AddForeignKey(
                name: "FK_Tasks_Peaople_AssignedById",
                table: "Tasks",
                column: "AssignedById",
                principalTable: "Peaople",
                principalColumn: "Id");

            migrationBuilder.AddForeignKey(
                name: "FK_Tasks_Peaople_AssignedToId",
                table: "Tasks",
                column: "AssignedToId",
                principalTable: "Peaople",
                principalColumn: "Id");
        }
    }
}
