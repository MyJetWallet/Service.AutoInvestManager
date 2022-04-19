using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Service.AutoInvestManager.Postgres.Migrations
{
    public partial class TotalValues : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<decimal>(
                name: "TotalFromAmount",
                schema: "recurringbuy",
                table: "instructions",
                type: "numeric",
                nullable: false,
                defaultValue: 0m);

            migrationBuilder.AddColumn<decimal>(
                name: "TotalToAmount",
                schema: "recurringbuy",
                table: "instructions",
                type: "numeric",
                nullable: false,
                defaultValue: 0m);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "TotalFromAmount",
                schema: "recurringbuy",
                table: "instructions");

            migrationBuilder.DropColumn(
                name: "TotalToAmount",
                schema: "recurringbuy",
                table: "instructions");
        }
    }
}
