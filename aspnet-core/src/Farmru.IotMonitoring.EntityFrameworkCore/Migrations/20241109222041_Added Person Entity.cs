using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Farmru.IotMonitoring.Migrations
{
    /// <inheritdoc />
    public partial class AddedPersonEntity : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Facilities_AbpUsers_PrimaryContactId",
                table: "Facilities");

            migrationBuilder.DropForeignKey(
                name: "FK_Incidents_AbpUsers_AssignedToId",
                table: "Incidents");

            migrationBuilder.DropForeignKey(
                name: "FK_Incidents_AbpUsers_CreatedById",
                table: "Incidents");

            migrationBuilder.DropForeignKey(
                name: "FK_Tasks_AbpUsers_AssignedById",
                table: "Tasks");

            migrationBuilder.DropForeignKey(
                name: "FK_Tasks_AbpUsers_AssignedToId",
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

            migrationBuilder.CreateTable(
                name: "Peaople",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    IdentityNumber = table.Column<string>(type: "nvarchar(13)", maxLength: 13, nullable: true),
                    Title = table.Column<int>(type: "int", nullable: true),
                    FirstName = table.Column<string>(type: "nvarchar(50)", maxLength: 50, nullable: true),
                    LastName = table.Column<string>(type: "nvarchar(50)", maxLength: 50, nullable: true),
                    MiddleName = table.Column<string>(type: "nvarchar(50)", maxLength: 50, nullable: true),
                    Biography = table.Column<string>(type: "nvarchar(max)", maxLength: 50000, nullable: true),
                    Initials = table.Column<string>(type: "nvarchar(10)", maxLength: 10, nullable: true),
                    CustomShortName = table.Column<string>(type: "nvarchar(60)", maxLength: 60, nullable: true),
                    HomeNumber = table.Column<string>(type: "nvarchar(20)", maxLength: 20, nullable: true),
                    MobileNumber1 = table.Column<string>(type: "nvarchar(20)", maxLength: 20, nullable: true),
                    MobileNumber2 = table.Column<string>(type: "nvarchar(20)", maxLength: 20, nullable: true),
                    EmailAddress1 = table.Column<string>(type: "nvarchar(100)", maxLength: 100, nullable: true),
                    EmailAddress2 = table.Column<string>(type: "nvarchar(100)", maxLength: 100, nullable: true),
                    DateOfBirth = table.Column<DateTime>(type: "datetime2", nullable: true),
                    Gender = table.Column<int>(type: "int", nullable: true),
                    FullName = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    UserId = table.Column<long>(type: "bigint", nullable: true),
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
                    table.PrimaryKey("PK_Peaople", x => x.Id);
                    table.ForeignKey(
                        name: "FK_Peaople_AbpUsers_UserId",
                        column: x => x.UserId,
                        principalTable: "AbpUsers",
                        principalColumn: "Id");
                });

            migrationBuilder.CreateIndex(
                name: "IX_Peaople_UserId",
                table: "Peaople",
                column: "UserId");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "Peaople");

            migrationBuilder.AddColumn<long>(
                name: "AssignedById",
                table: "Tasks",
                type: "bigint",
                nullable: true);

            migrationBuilder.AddColumn<long>(
                name: "AssignedToId",
                table: "Tasks",
                type: "bigint",
                nullable: true);

            migrationBuilder.AddColumn<long>(
                name: "AssignedToId",
                table: "Incidents",
                type: "bigint",
                nullable: true);

            migrationBuilder.AddColumn<long>(
                name: "CreatedById",
                table: "Incidents",
                type: "bigint",
                nullable: true);

            migrationBuilder.AddColumn<long>(
                name: "PrimaryContactId",
                table: "Facilities",
                type: "bigint",
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
                name: "FK_Facilities_AbpUsers_PrimaryContactId",
                table: "Facilities",
                column: "PrimaryContactId",
                principalTable: "AbpUsers",
                principalColumn: "Id");

            migrationBuilder.AddForeignKey(
                name: "FK_Incidents_AbpUsers_AssignedToId",
                table: "Incidents",
                column: "AssignedToId",
                principalTable: "AbpUsers",
                principalColumn: "Id");

            migrationBuilder.AddForeignKey(
                name: "FK_Incidents_AbpUsers_CreatedById",
                table: "Incidents",
                column: "CreatedById",
                principalTable: "AbpUsers",
                principalColumn: "Id");

            migrationBuilder.AddForeignKey(
                name: "FK_Tasks_AbpUsers_AssignedById",
                table: "Tasks",
                column: "AssignedById",
                principalTable: "AbpUsers",
                principalColumn: "Id");

            migrationBuilder.AddForeignKey(
                name: "FK_Tasks_AbpUsers_AssignedToId",
                table: "Tasks",
                column: "AssignedToId",
                principalTable: "AbpUsers",
                principalColumn: "Id");
        }
    }
}
