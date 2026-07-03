using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace LunaWash.DAL.Migrations
{
    /// <inheritdoc />
    public partial class AddStaffProfile : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "StaffProfiles",
                columns: table => new
                {
                    UserId = table.Column<string>(type: "varchar(50)", nullable: false),
                    Salary = table.Column<decimal>(type: "decimal(18,2)", nullable: false),
                    LeaveDays = table.Column<int>(type: "int", nullable: false),
                    UsedLeaveDays = table.Column<int>(type: "int", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_StaffProfiles", x => x.UserId);
                    table.ForeignKey(
                        name: "FK_StaffProfiles_Users_UserId",
                        column: x => x.UserId,
                        principalTable: "Users",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "StaffProfiles");
        }
    }
}
