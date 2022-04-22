using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Service.AutoInvestManager.Postgres.Migrations
{
    public partial class AvgPrice : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<decimal>(
                name: "AvgPrice",
                schema: "recurringbuy",
                table: "instructions",
                type: "numeric",
                nullable: false,
                defaultValue: 0m);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "AvgPrice",
                schema: "recurringbuy",
                table: "instructions");
        }
    }
}
