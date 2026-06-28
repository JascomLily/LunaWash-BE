using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace LunaWash.DAL.Migrations
{
    /// <inheritdoc />
    public partial class AddServiceReviewV2 : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {








            migrationBuilder.CreateTable(
                name: "ServiceReviews",
                columns: table => new
                {
                    Id = table.Column<string>(type: "nvarchar(50)", maxLength: 50, nullable: false),
                    BookingId = table.Column<string>(type: "varchar(50)", maxLength: 50, nullable: false),
                    OverallRating = table.Column<double>(type: "float", nullable: false),
                    CleanlinessRating = table.Column<int>(type: "int", nullable: false),
                    SpeedRating = table.Column<int>(type: "int", nullable: false),
                    StaffRating = table.Column<int>(type: "int", nullable: false),
                    Comment = table.Column<string>(type: "nvarchar(1000)", maxLength: 1000, nullable: true),
                    CreatedAt = table.Column<DateTime>(type: "datetime2", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_ServiceReviews", x => x.Id);
                    table.ForeignKey(
                        name: "FK_ServiceReviews_Bookings_BookingId",
                        column: x => x.BookingId,
                        principalTable: "Bookings",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });



            migrationBuilder.CreateIndex(
                name: "IX_ServiceReviews_BookingId",
                table: "ServiceReviews",
                column: "BookingId",
                unique: true);


        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_PointHistories_CustomerProfiles_UserId",
                table: "PointHistories");

            migrationBuilder.DropTable(
                name: "ServiceReviews");

            migrationBuilder.DropIndex(
                name: "IX_PointHistories_UserId",
                table: "PointHistories");

            migrationBuilder.DropColumn(
                name: "Address",
                table: "Users");

            migrationBuilder.DropColumn(
                name: "BranchId",
                table: "Users");

            migrationBuilder.DropColumn(
                name: "CheckoutTime",
                table: "Bookings");

            migrationBuilder.AlterColumn<string>(
                name: "UserId",
                table: "PointHistories",
                type: "nvarchar(max)",
                nullable: false,
                oldClrType: typeof(string),
                oldType: "varchar(50)");

            migrationBuilder.AddColumn<string>(
                name: "CustomerProfileUserId",
                table: "PointHistories",
                type: "varchar(50)",
                nullable: false,
                defaultValue: "");

            migrationBuilder.CreateIndex(
                name: "IX_PointHistories_CustomerProfileUserId",
                table: "PointHistories",
                column: "CustomerProfileUserId");

            migrationBuilder.AddForeignKey(
                name: "FK_PointHistories_CustomerProfiles_CustomerProfileUserId",
                table: "PointHistories",
                column: "CustomerProfileUserId",
                principalTable: "CustomerProfiles",
                principalColumn: "UserId",
                onDelete: ReferentialAction.Cascade);
        }
    }
}
