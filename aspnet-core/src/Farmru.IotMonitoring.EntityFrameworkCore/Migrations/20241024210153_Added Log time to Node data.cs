using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Farmru.IotMonitoring.Migrations
{
    /// <inheritdoc />
    public partial class AddedLogtimetoNodedata : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<DateTime>(
                name: "LoggingTime",
                table: "NodeDatas",
                type: "datetime2",
                nullable: true);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "LoggingTime",
                table: "NodeDatas");
        }
    }
}
