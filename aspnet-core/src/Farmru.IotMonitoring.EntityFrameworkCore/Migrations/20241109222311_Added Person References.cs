using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Farmru.IotMonitoring.Migrations
{
    /// <inheritdoc />
    public partial class AddedPersonReferences : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<Guid>(
                name: "AssignedById",
                table: "Tasks",
                type: "uniqueidentifier",
                nullable: true);

            migrationBuilder.AddColumn<Guid>(
                name: "AssignedToId",
                table: "Tasks",
                type: "uniqueidentifier",
                nullable: true);

            migrationBuilder.AddColumn<Guid>(
                name: "AssignedToId",
                table: "Incidents",
                type: "uniqueidentifier",
                nullable: true);

            migrationBuilder.AddColumn<Guid>(
                name: "CreatedById",
                table: "Incidents",
                type: "uniqueidentifier",
                nullable: true);

            migrationBuilder.AddColumn<Guid>(
                name: "PrimaryContactId",
                table: "Facilities",
                type: "uniqueidentifier",
                nullable: true);

            migrationBuilder.CreateIndex(
                name: "IX_Tasks_AssignedById",
                table: "Tasks",
                column: "AssignedById");

            migrationBuilder.CreateIndex(
                name: "IX_Tasks_AssignedToId",
                table: "Tasks",
                column: "AssignedToId");

            migrationBuilder.CreateIndex(
                name: "IX_Incidents_AssignedToId",
                table: "Incidents",
                column: "AssignedToId");

            migrationBuilder.CreateIndex(
                name: "IX_Incidents_CreatedById",
                table: "Incidents",
                column: "CreatedById");

            migrationBuilder.CreateIndex(
                name: "IX_Facilities_PrimaryContactId",
                table: "Facilities",
                column: "PrimaryContactId");

            migrationBuilder.AddForeignKey(
                name: "FK_Facilities_Peaople_PrimaryContactId",
                table: "Facilities",
                column: "PrimaryContactId",
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

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Facilities_Peaople_PrimaryContactId",
                table: "Facilities");

            migrationBuilder.DropForeignKey(
                name: "FK_Incidents_Peaople_AssignedToId",
                table: "Incidents");

            migrationBuilder.DropForeignKey(
                name: "FK_Incidents_Peaople_CreatedById",
                table: "Incidents");

            migrationBuilder.DropForeignKey(
                name: "FK_Tasks_Peaople_AssignedById",
                table: "Tasks");

            migrationBuilder.DropForeignKey(
                name: "FK_Tasks_Peaople_AssignedToId",
                table: "Tasks");

            migrationBuilder.DropIndex(
                name: "IX_Tasks_AssignedById",
                table: "Tasks");

            migrationBuilder.DropIndex(
                name: "IX_Tasks_AssignedToId",
                table: "Tasks");

            migrationBuilder.DropIndex(
                name: "IX_Incidents_AssignedToId",
                table: "Incidents");

            migrationBuilder.DropIndex(
                name: "IX_Incidents_CreatedById",
                table: "Incidents");

            migrationBuilder.DropIndex(
                name: "IX_Facilities_PrimaryContactId",
                table: "Facilities");

            migrationBuilder.DropColumn(
                name: "AssignedById",
                table: "Tasks");

            migrationBuilder.DropColumn(
                name: "AssignedToId",
                table: "Tasks");

            migrationBuilder.DropColumn(
                name: "AssignedToId",
                table: "Incidents");

            migrationBuilder.DropColumn(
                name: "CreatedById",
                table: "Incidents");

            migrationBuilder.DropColumn(
                name: "PrimaryContactId",
                table: "Facilities");
        }
    }
}
