using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Service.AutoInvestManager.Postgres.Migrations
{
    public partial class OrderCreationTime : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<DateTime>(
                name: "CreationTime",
                schema: "recurringbuy",
                table: "orders",
                type: "timestamp with time zone",
                nullable: false,
                defaultValue: new DateTime(1, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified));
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "CreationTime",
                schema: "recurringbuy",
                table: "orders");
        }
    }
}
