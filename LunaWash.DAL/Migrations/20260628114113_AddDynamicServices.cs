using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace LunaWash.DAL.Migrations
{
    /// <inheritdoc />
    public partial class AddDynamicServices : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<string>(
                name: "IconName",
                table: "WashServices",
                type: "nvarchar(100)",
                maxLength: 100,
                nullable: true);

            migrationBuilder.AddColumn<bool>(
                name: "IsPopular",
                table: "WashServices",
                type: "bit",
                nullable: false,
                defaultValue: false);

            migrationBuilder.AddColumn<string>(
                name: "ServiceType",
                table: "WashServices",
                type: "nvarchar(50)",
                maxLength: 50,
                nullable: false,
                defaultValue: "Package");

            migrationBuilder.AddColumn<string>(
                name: "BranchId",
                table: "ServiceReviews",
                type: "nvarchar(50)",
                maxLength: 50,
                nullable: false,
                defaultValue: "");

            migrationBuilder.AddColumn<string>(
                name: "CustomerId",
                table: "ServiceReviews",
                type: "nvarchar(50)",
                maxLength: 50,
                nullable: false,
                defaultValue: "");

            migrationBuilder.CreateTable(
                name: "BookingServices",
                columns: table => new
                {
                    Id = table.Column<string>(type: "varchar(50)", unicode: false, maxLength: 50, nullable: false),
                    BookingId = table.Column<string>(type: "varchar(50)", unicode: false, maxLength: 50, nullable: false),
                    WashServiceId = table.Column<string>(type: "varchar(50)", unicode: false, maxLength: 50, nullable: false),
                    PriceAtTime = table.Column<decimal>(type: "decimal(18,2)", nullable: false),
                    DurationAtTime = table.Column<int>(type: "int", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_BookingServices", x => x.Id);
                    table.ForeignKey(
                        name: "FK_BookingServices_Bookings_BookingId",
                        column: x => x.BookingId,
                        principalTable: "Bookings",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_BookingServices_WashServices_WashServiceId",
                        column: x => x.WashServiceId,
                        principalTable: "WashServices",
                        principalColumn: "Id");
                });

            migrationBuilder.CreateTable(
                name: "Equipments",
                columns: table => new
                {
                    Id = table.Column<string>(type: "nvarchar(50)", maxLength: 50, nullable: false),
                    BranchId = table.Column<string>(type: "varchar(50)", maxLength: 50, nullable: false),
                    Name = table.Column<string>(type: "nvarchar(150)", maxLength: 150, nullable: false),
                    Category = table.Column<string>(type: "nvarchar(100)", maxLength: 100, nullable: false),
                    Status = table.Column<string>(type: "nvarchar(50)", maxLength: 50, nullable: false),
                    Priority = table.Column<string>(type: "nvarchar(50)", maxLength: 50, nullable: false),
                    LastMaintenance = table.Column<string>(type: "nvarchar(50)", maxLength: 50, nullable: true),
                    NextMaintenance = table.Column<string>(type: "nvarchar(50)", maxLength: 50, nullable: true),
                    CreatedAt = table.Column<DateTime>(type: "datetime2", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Equipments", x => x.Id);
                    table.ForeignKey(
                        name: "FK_Equipments_Branches_BranchId",
                        column: x => x.BranchId,
                        principalTable: "Branches",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "ServiceFeatures",
                columns: table => new
                {
                    Id = table.Column<string>(type: "varchar(50)", unicode: false, maxLength: 50, nullable: false),
                    WashServiceId = table.Column<string>(type: "varchar(50)", unicode: false, maxLength: 50, nullable: false),
                    FeatureText = table.Column<string>(type: "nvarchar(250)", maxLength: 250, nullable: false),
                    DisplayOrder = table.Column<int>(type: "int", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_ServiceFeatures", x => x.Id);
                    table.ForeignKey(
                        name: "FK_ServiceFeatures_WashServices_WashServiceId",
                        column: x => x.WashServiceId,
                        principalTable: "WashServices",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "MaintenanceTasks",
                columns: table => new
                {
                    Id = table.Column<string>(type: "nvarchar(50)", maxLength: 50, nullable: false),
                    EquipmentId = table.Column<string>(type: "nvarchar(50)", maxLength: 50, nullable: false),
                    BranchId = table.Column<string>(type: "varchar(50)", maxLength: 50, nullable: false),
                    TaskName = table.Column<string>(type: "nvarchar(200)", maxLength: 200, nullable: false),
                    Description = table.Column<string>(type: "nvarchar(500)", maxLength: 500, nullable: true),
                    Status = table.Column<string>(type: "nvarchar(50)", maxLength: 50, nullable: false),
                    CreatedAt = table.Column<DateTime>(type: "datetime2", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_MaintenanceTasks", x => x.Id);
                    table.ForeignKey(
                        name: "FK_MaintenanceTasks_Branches_BranchId",
                        column: x => x.BranchId,
                        principalTable: "Branches",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_MaintenanceTasks_Equipments_EquipmentId",
                        column: x => x.EquipmentId,
                        principalTable: "Equipments",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateIndex(
                name: "IX_BookingServices_BookingId",
                table: "BookingServices",
                column: "BookingId");

            migrationBuilder.CreateIndex(
                name: "IX_BookingServices_WashServiceId",
                table: "BookingServices",
                column: "WashServiceId");

            migrationBuilder.CreateIndex(
                name: "IX_Equipments_BranchId",
                table: "Equipments",
                column: "BranchId");

            migrationBuilder.CreateIndex(
                name: "IX_MaintenanceTasks_BranchId",
                table: "MaintenanceTasks",
                column: "BranchId");

            migrationBuilder.CreateIndex(
                name: "IX_MaintenanceTasks_EquipmentId",
                table: "MaintenanceTasks",
                column: "EquipmentId");

            migrationBuilder.CreateIndex(
                name: "IX_ServiceFeatures_WashServiceId",
                table: "ServiceFeatures",
                column: "WashServiceId");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "BookingServices");

            migrationBuilder.DropTable(
                name: "MaintenanceTasks");

            migrationBuilder.DropTable(
                name: "ServiceFeatures");

            migrationBuilder.DropTable(
                name: "Equipments");

            migrationBuilder.DropColumn(
                name: "IconName",
                table: "WashServices");

            migrationBuilder.DropColumn(
                name: "IsPopular",
                table: "WashServices");

            migrationBuilder.DropColumn(
                name: "ServiceType",
                table: "WashServices");

            migrationBuilder.DropColumn(
                name: "BranchId",
                table: "ServiceReviews");

            migrationBuilder.DropColumn(
                name: "CustomerId",
                table: "ServiceReviews");
        }
    }
}
