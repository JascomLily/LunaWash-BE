using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace LunaWash.DAL.Migrations
{
    /// <inheritdoc />
    public partial class UpdateMembershipAndAddReviewsResponseSettings : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<DateTime>(
                name: "RespondedAt",
                table: "ServiceReviews",
                type: "datetime2",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "RespondedById",
                table: "ServiceReviews",
                type: "varchar(50)",
                maxLength: 50,
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "ResponseText",
                table: "ServiceReviews",
                type: "nvarchar(1000)",
                maxLength: 1000,
                nullable: true);

            migrationBuilder.AddColumn<int>(
                name: "AdvanceBookingDays",
                table: "MembershipTiers",
                type: "int",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.AddColumn<int>(
                name: "KeepPoints",
                table: "MembershipTiers",
                type: "int",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.CreateTable(
                name: "SystemSettings",
                columns: table => new
                {
                    Id = table.Column<string>(type: "nvarchar(100)", maxLength: 100, nullable: false),
                    Value = table.Column<string>(type: "nvarchar(500)", maxLength: 500, nullable: false),
                    Description = table.Column<string>(type: "nvarchar(500)", maxLength: 500, nullable: true),
                    UpdatedAt = table.Column<DateTime>(type: "datetime2", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_SystemSettings", x => x.Id);
                });

            migrationBuilder.CreateIndex(
                name: "IX_ServiceReviews_RespondedById",
                table: "ServiceReviews",
                column: "RespondedById");

            migrationBuilder.AddForeignKey(
                name: "FK_ServiceReviews_Users_RespondedById",
                table: "ServiceReviews",
                column: "RespondedById",
                principalTable: "Users",
                principalColumn: "Id");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_ServiceReviews_Users_RespondedById",
                table: "ServiceReviews");

            migrationBuilder.DropTable(
                name: "SystemSettings");

            migrationBuilder.DropIndex(
                name: "IX_ServiceReviews_RespondedById",
                table: "ServiceReviews");

            migrationBuilder.DropColumn(
                name: "RespondedAt",
                table: "ServiceReviews");

            migrationBuilder.DropColumn(
                name: "RespondedById",
                table: "ServiceReviews");

            migrationBuilder.DropColumn(
                name: "ResponseText",
                table: "ServiceReviews");

            migrationBuilder.DropColumn(
                name: "AdvanceBookingDays",
                table: "MembershipTiers");

            migrationBuilder.DropColumn(
                name: "KeepPoints",
                table: "MembershipTiers");
        }
    }
}
