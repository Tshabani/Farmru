using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Farmru.IotMonitoring.Migrations
{
    /// <inheritdoc />
    public partial class AddedDefaultproptoFacility : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {

            migrationBuilder.AlterColumn<string>(
               name: "AltEmailAddress",
               table: "People",
               type: "nvarchar(100)", // Keep 100 or set a more reasonable value
               maxLength: 100,
               nullable: true,
               oldClrType: typeof(string),
               oldType: "nvarchar(100)",
               oldMaxLength: 100,
               oldNullable: true);

            migrationBuilder.AddColumn<bool>(
                name: "IsDefault",
                table: "Facilities",
                type: "bit",
                nullable: true);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "IsDefault",
                table: "Facilities");

            migrationBuilder.AlterColumn<string>(
                name: "AltEmailAddress",
                table: "People",
                type: "nvarchar(100)",
                maxLength: 100,
                nullable: true,
                oldClrType: typeof(string),
                oldType: "nvarchar(10)",
                oldMaxLength: 10,
                oldNullable: true);
        }
    }
}
