using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Service.AutoInvestManager.Postgres.Migrations
{
    public partial class Fees : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<decimal>(
                name: "FeeAmount",
                schema: "recurringbuy",
                table: "orders",
                type: "numeric",
                nullable: false,
                defaultValue: 0m);

            migrationBuilder.AddColumn<string>(
                name: "FeeAsset",
                schema: "recurringbuy",
                table: "orders",
                type: "text",
                nullable: true);

            migrationBuilder.AddColumn<decimal>(
                name: "FeeCoef",
                schema: "recurringbuy",
                table: "orders",
                type: "numeric",
                nullable: false,
                defaultValue: 0m);

            migrationBuilder.AddColumn<int>(
                name: "ScheduleType",
                schema: "recurringbuy",
                table: "orders",
                type: "integer",
                nullable: false,
                defaultValue: 0);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "FeeAmount",
                schema: "recurringbuy",
                table: "orders");

            migrationBuilder.DropColumn(
                name: "FeeAsset",
                schema: "recurringbuy",
                table: "orders");

            migrationBuilder.DropColumn(
                name: "FeeCoef",
                schema: "recurringbuy",
                table: "orders");

            migrationBuilder.DropColumn(
                name: "ScheduleType",
                schema: "recurringbuy",
                table: "orders");
        }
    }
}
