using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace LunaWash.DAL.Migrations
{
    /// <inheritdoc />
    public partial class UpdateMaintenanceAndAddIncidentsLogs : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<string>(
                name: "AssignedToId",
                table: "MaintenanceTasks",
                type: "varchar(50)",
                maxLength: 50,
                nullable: true);

            migrationBuilder.AddColumn<bool>(
                name: "IsIncident",
                table: "MaintenanceTasks",
                type: "bit",
                nullable: false,
                defaultValue: false);

            migrationBuilder.AddColumn<string>(
                name: "Priority",
                table: "MaintenanceTasks",
                type: "nvarchar(50)",
                maxLength: 50,
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "Resolution",
                table: "MaintenanceTasks",
                type: "nvarchar(500)",
                maxLength: 500,
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "SupportRequest",
                table: "MaintenanceTasks",
                type: "nvarchar(500)",
                maxLength: 500,
                nullable: true);

            migrationBuilder.AddColumn<DateTime>(
                name: "UpdatedAt",
                table: "MaintenanceTasks",
                type: "datetime2",
                nullable: true);

            migrationBuilder.CreateTable(
                name: "EquipmentCheckLogs",
                columns: table => new
                {
                    Id = table.Column<string>(type: "nvarchar(50)", maxLength: 50, nullable: false),
                    BranchId = table.Column<string>(type: "varchar(50)", maxLength: 50, nullable: false),
                    EquipmentId = table.Column<string>(type: "nvarchar(50)", maxLength: 50, nullable: false),
                    TechnicianId = table.Column<string>(type: "varchar(50)", maxLength: 50, nullable: false),
                    CheckTime = table.Column<DateTime>(type: "datetime2", nullable: false),
                    Condition = table.Column<string>(type: "nvarchar(50)", maxLength: 50, nullable: false),
                    Notes = table.Column<string>(type: "nvarchar(500)", maxLength: 500, nullable: true),
                    CreatedAt = table.Column<DateTime>(type: "datetime2", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_EquipmentCheckLogs", x => x.Id);
                    table.ForeignKey(
                        name: "FK_EquipmentCheckLogs_Branches_BranchId",
                        column: x => x.BranchId,
                        principalTable: "Branches",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_EquipmentCheckLogs_Equipments_EquipmentId",
                        column: x => x.EquipmentId,
                        principalTable: "Equipments",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_EquipmentCheckLogs_Users_TechnicianId",
                        column: x => x.TechnicianId,
                        principalTable: "Users",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateTable(
                name: "IncidentReports",
                columns: table => new
                {
                    Id = table.Column<string>(type: "nvarchar(50)", maxLength: 50, nullable: false),
                    BranchId = table.Column<string>(type: "varchar(50)", maxLength: 50, nullable: false),
                    EquipmentId = table.Column<string>(type: "nvarchar(50)", maxLength: 50, nullable: false),
                    ReporterId = table.Column<string>(type: "varchar(50)", maxLength: 50, nullable: false),
                    Title = table.Column<string>(type: "nvarchar(200)", maxLength: 200, nullable: false),
                    Description = table.Column<string>(type: "nvarchar(500)", maxLength: 500, nullable: true),
                    Status = table.Column<string>(type: "nvarchar(50)", maxLength: 50, nullable: false),
                    CreatedAt = table.Column<DateTime>(type: "datetime2", nullable: false),
                    UpdatedAt = table.Column<DateTime>(type: "datetime2", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_IncidentReports", x => x.Id);
                    table.ForeignKey(
                        name: "FK_IncidentReports_Branches_BranchId",
                        column: x => x.BranchId,
                        principalTable: "Branches",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_IncidentReports_Equipments_EquipmentId",
                        column: x => x.EquipmentId,
                        principalTable: "Equipments",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_IncidentReports_Users_ReporterId",
                        column: x => x.ReporterId,
                        principalTable: "Users",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateIndex(
                name: "IX_MaintenanceTasks_AssignedToId",
                table: "MaintenanceTasks",
                column: "AssignedToId");

            migrationBuilder.CreateIndex(
                name: "IX_EquipmentCheckLogs_BranchId",
                table: "EquipmentCheckLogs",
                column: "BranchId");

            migrationBuilder.CreateIndex(
                name: "IX_EquipmentCheckLogs_EquipmentId",
                table: "EquipmentCheckLogs",
                column: "EquipmentId");

            migrationBuilder.CreateIndex(
                name: "IX_EquipmentCheckLogs_TechnicianId",
                table: "EquipmentCheckLogs",
                column: "TechnicianId");

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
                name: "FK_MaintenanceTasks_Users_AssignedToId",
                table: "MaintenanceTasks",
                column: "AssignedToId",
                principalTable: "Users",
                principalColumn: "Id");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_MaintenanceTasks_Users_AssignedToId",
                table: "MaintenanceTasks");

            migrationBuilder.DropTable(
                name: "EquipmentCheckLogs");

            migrationBuilder.DropTable(
                name: "IncidentReports");

            migrationBuilder.DropIndex(
                name: "IX_MaintenanceTasks_AssignedToId",
                table: "MaintenanceTasks");

            migrationBuilder.DropColumn(
                name: "AssignedToId",
                table: "MaintenanceTasks");

            migrationBuilder.DropColumn(
                name: "IsIncident",
                table: "MaintenanceTasks");

            migrationBuilder.DropColumn(
                name: "Priority",
                table: "MaintenanceTasks");

            migrationBuilder.DropColumn(
                name: "Resolution",
                table: "MaintenanceTasks");

            migrationBuilder.DropColumn(
                name: "SupportRequest",
                table: "MaintenanceTasks");

            migrationBuilder.DropColumn(
                name: "UpdatedAt",
                table: "MaintenanceTasks");
        }
    }
}
