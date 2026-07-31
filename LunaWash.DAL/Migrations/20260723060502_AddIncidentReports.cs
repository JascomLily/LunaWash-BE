using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace LunaWash.DAL.Migrations
{
    /// <inheritdoc />
    public partial class AddIncidentReports : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<string>(
                name: "AssigneeId",
                table: "MaintenanceTasks",
                type: "varchar(50)",
                maxLength: 50,
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "IncidentReportId",
                table: "MaintenanceTasks",
                type: "nvarchar(50)",
                maxLength: 50,
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "Priority",
                table: "MaintenanceTasks",
                type: "nvarchar(50)",
                maxLength: 50,
                nullable: false,
                defaultValue: "");

            migrationBuilder.AddColumn<string>(
                name: "ReviewNote",
                table: "MaintenanceTasks",
                type: "nvarchar(1000)",
                maxLength: 1000,
                nullable: true);

            migrationBuilder.CreateTable(
                name: "IncidentReports",
                columns: table => new
                {
                    Id = table.Column<string>(type: "nvarchar(50)", maxLength: 50, nullable: false),
                    Title = table.Column<string>(type: "nvarchar(200)", maxLength: 200, nullable: false),
                    EquipmentId = table.Column<string>(type: "nvarchar(50)", maxLength: 50, nullable: true),
                    BranchId = table.Column<string>(type: "varchar(50)", maxLength: 50, nullable: false),
                    ReporterId = table.Column<string>(type: "varchar(50)", maxLength: 50, nullable: false),
                    Description = table.Column<string>(type: "nvarchar(1000)", maxLength: 1000, nullable: false),
                    Status = table.Column<string>(type: "nvarchar(50)", maxLength: 50, nullable: false),
                    ImageUrl = table.Column<string>(type: "nvarchar(500)", maxLength: 500, nullable: true),
                    CreatedAt = table.Column<DateTime>(type: "datetime2", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_IncidentReports", x => x.Id);
                    table.ForeignKey(
                        name: "FK_IncidentReports_Branches_BranchId",
                        column: x => x.BranchId,
                        principalTable: "Branches",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_IncidentReports_Equipments_EquipmentId",
                        column: x => x.EquipmentId,
                        principalTable: "Equipments",
                        principalColumn: "Id");
                    table.ForeignKey(
                        name: "FK_IncidentReports_Users_ReporterId",
                        column: x => x.ReporterId,
                        principalTable: "Users",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateIndex(
                name: "IX_MaintenanceTasks_AssigneeId",
                table: "MaintenanceTasks",
                column: "AssigneeId");

            migrationBuilder.CreateIndex(
                name: "IX_MaintenanceTasks_IncidentReportId",
                table: "MaintenanceTasks",
                column: "IncidentReportId");

            migrationBuilder.CreateIndex(
                name: "IX_IncidentReports_BranchId",
                table: "IncidentReports",
                column: "BranchId");

            migrationBuilder.CreateIndex(
                name: "IX_IncidentReports_EquipmentId",
                table: "IncidentReports",
                column: "EquipmentId");

            migrationBuilder.CreateIndex(
                name: "IX_IncidentReports_ReporterId",
                table: "IncidentReports",
                column: "ReporterId");

            migrationBuilder.AddForeignKey(
                name: "FK_MaintenanceTasks_IncidentReports_IncidentReportId",
                table: "MaintenanceTasks",
                column: "IncidentReportId",
                principalTable: "IncidentReports",
                principalColumn: "Id");

            migrationBuilder.AddForeignKey(
                name: "FK_MaintenanceTasks_Users_AssigneeId",
                table: "MaintenanceTasks",
                column: "AssigneeId",
                principalTable: "Users",
                principalColumn: "Id");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_MaintenanceTasks_IncidentReports_IncidentReportId",
                table: "MaintenanceTasks");

            migrationBuilder.DropForeignKey(
                name: "FK_MaintenanceTasks_Users_AssigneeId",
                table: "MaintenanceTasks");

            migrationBuilder.DropTable(
                name: "IncidentReports");

            migrationBuilder.DropIndex(
                name: "IX_MaintenanceTasks_AssigneeId",
                table: "MaintenanceTasks");

            migrationBuilder.DropIndex(
                name: "IX_MaintenanceTasks_IncidentReportId",
                table: "MaintenanceTasks");

            migrationBuilder.DropColumn(
                name: "AssigneeId",
                table: "MaintenanceTasks");

            migrationBuilder.DropColumn(
                name: "IncidentReportId",
                table: "MaintenanceTasks");

            migrationBuilder.DropColumn(
                name: "Priority",
                table: "MaintenanceTasks");

            migrationBuilder.DropColumn(
                name: "ReviewNote",
                table: "MaintenanceTasks");
        }
    }
}
