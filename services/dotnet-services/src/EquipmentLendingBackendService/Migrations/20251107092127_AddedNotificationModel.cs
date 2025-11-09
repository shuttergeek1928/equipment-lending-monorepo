using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace EquipmentLendingBackendService.Migrations
{
    /// <inheritdoc />
    public partial class AddedNotificationModel : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AlterColumn<bool>(
                name: "IsAvailable",
                table: "Equipments",
                type: "boolean",
                nullable: false,
                defaultValue: true,
                oldClrType: typeof(bool),
                oldType: "boolean",
                oldNullable: true,
                oldDefaultValue: true);

            migrationBuilder.AddColumn<bool>(
                name: "IsOverdueNotified",
                table: "BorrowingsAndReturns",
                type: "boolean",
                nullable: false,
                defaultValue: false);

            migrationBuilder.AddColumn<DateTimeOffset>(
                name: "LastNotifiedAt",
                table: "BorrowingsAndReturns",
                type: "timestamp with time zone",
                nullable: true);

            migrationBuilder.AddColumn<int>(
                name: "NotificationAttempts",
                table: "BorrowingsAndReturns",
                type: "integer",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.CreateTable(
                name: "Notifications",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uuid", nullable: false),
                    BorrowRequestId = table.Column<int>(type: "integer", nullable: false),
                    NotifiedAt = table.Column<DateTimeOffset>(type: "timestamp with time zone", nullable: false),
                    Channel = table.Column<string>(type: "text", nullable: false),
                    Status = table.Column<string>(type: "text", nullable: false),
                    Attempts = table.Column<int>(type: "integer", nullable: false),
                    Message = table.Column<string>(type: "text", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Notifications", x => x.Id);
                    table.ForeignKey(
                        name: "FK_Notifications_BorrowingsAndReturns_Id",
                        column: x => x.Id,
                        principalTable: "BorrowingsAndReturns",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "Notifications");

            migrationBuilder.DropColumn(
                name: "IsOverdueNotified",
                table: "BorrowingsAndReturns");

            migrationBuilder.DropColumn(
                name: "LastNotifiedAt",
                table: "BorrowingsAndReturns");

            migrationBuilder.DropColumn(
                name: "NotificationAttempts",
                table: "BorrowingsAndReturns");

            migrationBuilder.AlterColumn<bool>(
                name: "IsAvailable",
                table: "Equipments",
                type: "boolean",
                nullable: true,
                defaultValue: true,
                oldClrType: typeof(bool),
                oldType: "boolean",
                oldDefaultValue: true);
        }
    }
}
