using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Service.AutoInvestManager.Postgres.Migrations
{
    public partial class DateTimeFix2 : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.RenameColumn(
                name: "ScheduledTime",
                schema: "autoinvest",
                table: "instructions",
                newName: "ScheduledDateTime");
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.RenameColumn(
                name: "ScheduledDateTime",
                schema: "autoinvest",
                table: "instructions",
                newName: "ScheduledTime");
        }
    }
}
