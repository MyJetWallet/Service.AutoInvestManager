using System;
using Microsoft.EntityFrameworkCore.Migrations;
using Npgsql.EntityFrameworkCore.PostgreSQL.Metadata;

#nullable disable

namespace Service.AutoInvestManager.Postgres.Migrations
{
    public partial class InitialCreate : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.EnsureSchema(
                name: "autoinvest");

            migrationBuilder.CreateTable(
                name: "audit",
                schema: "autoinvest",
                columns: table => new
                {
                    LogId = table.Column<long>(type: "bigint", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    LogTimestamp = table.Column<DateTime>(type: "timestamp with time zone", nullable: false, defaultValue: new DateTime(1, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc)),
                    InstructionId = table.Column<string>(type: "text", nullable: true),
                    ClientId = table.Column<string>(type: "text", nullable: true),
                    BrokerId = table.Column<string>(type: "text", nullable: true),
                    WalletId = table.Column<string>(type: "text", nullable: true),
                    FromAmount = table.Column<decimal>(type: "numeric", nullable: false),
                    FromAsset = table.Column<string>(type: "text", nullable: true),
                    ToAsset = table.Column<string>(type: "text", nullable: true),
                    Status = table.Column<int>(type: "integer", nullable: false),
                    ScheduleType = table.Column<int>(type: "integer", nullable: false),
                    ScheduledTime = table.Column<TimeOnly>(type: "time without time zone", nullable: false),
                    ScheduledDayOfWeek = table.Column<int>(type: "integer", nullable: false),
                    ScheduledDayOfMonth = table.Column<int>(type: "integer", nullable: false),
                    CreationTime = table.Column<DateTime>(type: "timestamp with time zone", nullable: false, defaultValue: new DateTime(1, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc)),
                    LastExecutionTime = table.Column<DateTime>(type: "timestamp with time zone", nullable: false, defaultValue: new DateTime(1, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc)),
                    ShouldSendFailEmail = table.Column<bool>(type: "boolean", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_audit", x => x.LogId);
                });

            migrationBuilder.CreateTable(
                name: "instructions",
                schema: "autoinvest",
                columns: table => new
                {
                    Id = table.Column<string>(type: "text", nullable: false),
                    ClientId = table.Column<string>(type: "text", nullable: true),
                    BrokerId = table.Column<string>(type: "text", nullable: true),
                    WalletId = table.Column<string>(type: "text", nullable: true),
                    FromAmount = table.Column<decimal>(type: "numeric", nullable: false),
                    FromAsset = table.Column<string>(type: "text", nullable: true),
                    ToAsset = table.Column<string>(type: "text", nullable: true),
                    Status = table.Column<int>(type: "integer", nullable: false),
                    ScheduleType = table.Column<int>(type: "integer", nullable: false),
                    ScheduledTime = table.Column<TimeOnly>(type: "time without time zone", nullable: false),
                    ScheduledDayOfWeek = table.Column<int>(type: "integer", nullable: false),
                    ScheduledDayOfMonth = table.Column<int>(type: "integer", nullable: false),
                    CreationTime = table.Column<DateTime>(type: "timestamp with time zone", nullable: false, defaultValue: new DateTime(1, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc)),
                    LastExecutionTime = table.Column<DateTime>(type: "timestamp with time zone", nullable: false, defaultValue: new DateTime(1, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc)),
                    ShouldSendFailEmail = table.Column<bool>(type: "boolean", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_instructions", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "orders",
                schema: "autoinvest",
                columns: table => new
                {
                    Id = table.Column<string>(type: "text", nullable: false),
                    ClientId = table.Column<string>(type: "text", nullable: true),
                    BrokerId = table.Column<string>(type: "text", nullable: true),
                    WalletId = table.Column<string>(type: "text", nullable: true),
                    InvestInstructionId = table.Column<string>(type: "text", nullable: true),
                    FromAmount = table.Column<decimal>(type: "numeric", nullable: false),
                    FromAsset = table.Column<string>(type: "text", nullable: true),
                    ToAmount = table.Column<decimal>(type: "numeric", nullable: false),
                    ToAsset = table.Column<string>(type: "text", nullable: true),
                    Price = table.Column<decimal>(type: "numeric", nullable: false),
                    Status = table.Column<int>(type: "integer", nullable: false),
                    ExecutionTime = table.Column<DateTime>(type: "timestamp with time zone", nullable: false, defaultValue: new DateTime(1, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc))
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_orders", x => x.Id);
                });

            migrationBuilder.CreateIndex(
                name: "IX_audit_ClientId",
                schema: "autoinvest",
                table: "audit",
                column: "ClientId");

            migrationBuilder.CreateIndex(
                name: "IX_audit_InstructionId",
                schema: "autoinvest",
                table: "audit",
                column: "InstructionId");

            migrationBuilder.CreateIndex(
                name: "IX_instructions_ClientId",
                schema: "autoinvest",
                table: "instructions",
                column: "ClientId");

            migrationBuilder.CreateIndex(
                name: "IX_instructions_Status",
                schema: "autoinvest",
                table: "instructions",
                column: "Status");

            migrationBuilder.CreateIndex(
                name: "IX_orders_ClientId",
                schema: "autoinvest",
                table: "orders",
                column: "ClientId");

            migrationBuilder.CreateIndex(
                name: "IX_orders_Status",
                schema: "autoinvest",
                table: "orders",
                column: "Status");
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "audit",
                schema: "autoinvest");

            migrationBuilder.DropTable(
                name: "instructions",
                schema: "autoinvest");

            migrationBuilder.DropTable(
                name: "orders",
                schema: "autoinvest");
        }
    }
}
