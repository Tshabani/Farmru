using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Farmru.IotMonitoring.Migrations
{
    /// <inheritdoc />
    public partial class Added_Fertilizer_Management : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "FertilizerProducts",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    TenantId = table.Column<int>(type: "int", nullable: false),
                    Name = table.Column<string>(type: "nvarchar(450)", nullable: true),
                    NitrogenPercent = table.Column<decimal>(type: "decimal(5,2)", precision: 5, scale: 2, nullable: false),
                    PhosphorusPercent = table.Column<decimal>(type: "decimal(5,2)", precision: 5, scale: 2, nullable: false),
                    PotassiumPercent = table.Column<decimal>(type: "decimal(5,2)", precision: 5, scale: 2, nullable: false),
                    UnitCostPerKg = table.Column<decimal>(type: "decimal(8,2)", precision: 8, scale: 2, nullable: true),
                    SupplierId = table.Column<Guid>(type: "uniqueidentifier", nullable: true),
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
                    table.PrimaryKey("PK_FertilizerProducts", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "NutrientBalanceSnapshots",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    TenantId = table.Column<int>(type: "int", nullable: false),
                    FieldId = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    SnapshotDate = table.Column<DateTime>(type: "datetime2", nullable: false),
                    SensedNitrogen = table.Column<decimal>(type: "decimal(6,2)", precision: 6, scale: 2, nullable: false),
                    SensedPhosphorus = table.Column<decimal>(type: "decimal(6,2)", precision: 6, scale: 2, nullable: false),
                    SensedPotassium = table.Column<decimal>(type: "decimal(6,2)", precision: 6, scale: 2, nullable: false),
                    AppliedNitrogenTrailing30d = table.Column<decimal>(type: "decimal(8,2)", precision: 8, scale: 2, nullable: false),
                    AppliedPhosphorusTrailing30d = table.Column<decimal>(type: "decimal(8,2)", precision: 8, scale: 2, nullable: false),
                    AppliedPotassiumTrailing30d = table.Column<decimal>(type: "decimal(8,2)", precision: 8, scale: 2, nullable: false),
                    NitrogenStatus = table.Column<int>(type: "int", nullable: false),
                    PhosphorusStatus = table.Column<int>(type: "int", nullable: false),
                    PotassiumStatus = table.Column<int>(type: "int", nullable: false),
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
                    table.PrimaryKey("PK_NutrientBalanceSnapshots", x => x.Id);
                    table.ForeignKey(
                        name: "FK_NutrientBalanceSnapshots_Fields_FieldId",
                        column: x => x.FieldId,
                        principalTable: "Fields",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "FertilizerApplications",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    TenantId = table.Column<int>(type: "int", nullable: false),
                    FieldId = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    CropSeasonId = table.Column<Guid>(type: "uniqueidentifier", nullable: true),
                    ProductId = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    RateKgPerHectare = table.Column<decimal>(type: "decimal(8,2)", precision: 8, scale: 2, nullable: false),
                    ApplicationDate = table.Column<DateTime>(type: "datetime2", nullable: false),
                    Cost = table.Column<decimal>(type: "decimal(10,2)", precision: 10, scale: 2, nullable: true),
                    OperatorPersonId = table.Column<Guid>(type: "uniqueidentifier", nullable: true),
                    InventorySourceRef = table.Column<Guid>(type: "uniqueidentifier", nullable: true),
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
                    table.PrimaryKey("PK_FertilizerApplications", x => x.Id);
                    table.ForeignKey(
                        name: "FK_FertilizerApplications_CropSeasons_CropSeasonId",
                        column: x => x.CropSeasonId,
                        principalTable: "CropSeasons",
                        principalColumn: "Id");
                    table.ForeignKey(
                        name: "FK_FertilizerApplications_FertilizerProducts_ProductId",
                        column: x => x.ProductId,
                        principalTable: "FertilizerProducts",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_FertilizerApplications_Fields_FieldId",
                        column: x => x.FieldId,
                        principalTable: "Fields",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_FertilizerApplications_People_OperatorPersonId",
                        column: x => x.OperatorPersonId,
                        principalTable: "People",
                        principalColumn: "Id");
                });

            migrationBuilder.CreateIndex(
                name: "IX_FertilizerApplications_CropSeasonId",
                table: "FertilizerApplications",
                column: "CropSeasonId");

            migrationBuilder.CreateIndex(
                name: "IX_FertilizerApplications_FieldId",
                table: "FertilizerApplications",
                column: "FieldId");

            migrationBuilder.CreateIndex(
                name: "IX_FertilizerApplications_OperatorPersonId",
                table: "FertilizerApplications",
                column: "OperatorPersonId");

            migrationBuilder.CreateIndex(
                name: "IX_FertilizerApplications_ProductId",
                table: "FertilizerApplications",
                column: "ProductId");

            migrationBuilder.CreateIndex(
                name: "IX_FertilizerApplications_TenantId_FieldId_ApplicationDate",
                table: "FertilizerApplications",
                columns: new[] { "TenantId", "FieldId", "ApplicationDate" });

            migrationBuilder.CreateIndex(
                name: "IX_FertilizerProducts_TenantId_Name",
                table: "FertilizerProducts",
                columns: new[] { "TenantId", "Name" });

            migrationBuilder.CreateIndex(
                name: "IX_NutrientBalanceSnapshots_FieldId",
                table: "NutrientBalanceSnapshots",
                column: "FieldId");

            migrationBuilder.CreateIndex(
                name: "IX_NutrientBalanceSnapshots_TenantId_FieldId_SnapshotDate",
                table: "NutrientBalanceSnapshots",
                columns: new[] { "TenantId", "FieldId", "SnapshotDate" });
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "FertilizerApplications");

            migrationBuilder.DropTable(
                name: "NutrientBalanceSnapshots");

            migrationBuilder.DropTable(
                name: "FertilizerProducts");
        }
    }
}
