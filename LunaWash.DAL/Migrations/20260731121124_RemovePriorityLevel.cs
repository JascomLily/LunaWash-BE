using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace LunaWash.DAL.Migrations
{
    /// <inheritdoc />
    public partial class RemovePriorityLevel : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "PriorityLevel",
                table: "MembershipTiers");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<int>(
                name: "PriorityLevel",
                table: "MembershipTiers",
                type: "int",
                nullable: false,
                defaultValue: 1);
        }
    }
}
