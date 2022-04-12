﻿// <auto-generated />
using System;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Infrastructure;
using Microsoft.EntityFrameworkCore.Storage.ValueConversion;
using Npgsql.EntityFrameworkCore.PostgreSQL.Metadata;
using Service.AutoInvestManager.Postgres;

#nullable disable

namespace Service.AutoInvestManager.Postgres.Migrations
{
    [DbContext(typeof(DatabaseContext))]
    partial class DatabaseContextModelSnapshot : ModelSnapshot
    {
        protected override void BuildModel(ModelBuilder modelBuilder)
        {
#pragma warning disable 612, 618
            modelBuilder
                .HasDefaultSchema("autoinvest")
                .HasAnnotation("ProductVersion", "6.0.4")
                .HasAnnotation("Relational:MaxIdentifierLength", 63);

            NpgsqlModelBuilderExtensions.UseIdentityByDefaultColumns(modelBuilder);

            modelBuilder.Entity("Service.AutoInvestManager.Domain.Models.InvestInstruction", b =>
                {
                    b.Property<string>("Id")
                        .HasColumnType("text");

                    b.Property<string>("BrokerId")
                        .HasColumnType("text");

                    b.Property<string>("ClientId")
                        .HasColumnType("text");

                    b.Property<DateTime>("CreationTime")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("timestamp with time zone")
                        .HasDefaultValue(new DateTime(1, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc));

                    b.Property<decimal>("FromAmount")
                        .HasColumnType("numeric");

                    b.Property<string>("FromAsset")
                        .HasColumnType("text");

                    b.Property<DateTime>("LastExecutionTime")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("timestamp with time zone")
                        .HasDefaultValue(new DateTime(1, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc));

                    b.Property<string>("OriginalQuoteId")
                        .HasColumnType("text");

                    b.Property<int>("ScheduleType")
                        .HasColumnType("integer");

                    b.Property<int>("ScheduledDayOfMonth")
                        .HasColumnType("integer");

                    b.Property<int>("ScheduledDayOfWeek")
                        .HasColumnType("integer");

                    b.Property<TimeOnly>("ScheduledTime")
                        .HasColumnType("time without time zone");

                    b.Property<bool>("ShouldSendFailEmail")
                        .HasColumnType("boolean");

                    b.Property<int>("Status")
                        .HasColumnType("integer");

                    b.Property<string>("ToAsset")
                        .HasColumnType("text");

                    b.Property<string>("WalletId")
                        .HasColumnType("text");

                    b.HasKey("Id");

                    b.HasIndex("ClientId");

                    b.HasIndex("Status");

                    b.ToTable("instructions", "autoinvest");
                });

            modelBuilder.Entity("Service.AutoInvestManager.Domain.Models.InvestInstructionAuditRecord", b =>
                {
                    b.Property<long>("LogId")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("bigint");

                    NpgsqlPropertyBuilderExtensions.UseIdentityByDefaultColumn(b.Property<long>("LogId"));

                    b.Property<string>("BrokerId")
                        .HasColumnType("text");

                    b.Property<string>("ClientId")
                        .HasColumnType("text");

                    b.Property<DateTime>("CreationTime")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("timestamp with time zone")
                        .HasDefaultValue(new DateTime(1, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc));

                    b.Property<decimal>("FromAmount")
                        .HasColumnType("numeric");

                    b.Property<string>("FromAsset")
                        .HasColumnType("text");

                    b.Property<string>("InstructionId")
                        .HasColumnType("text");

                    b.Property<DateTime>("LastExecutionTime")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("timestamp with time zone")
                        .HasDefaultValue(new DateTime(1, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc));

                    b.Property<DateTime>("LogTimestamp")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("timestamp with time zone")
                        .HasDefaultValue(new DateTime(1, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc));

                    b.Property<int>("ScheduleType")
                        .HasColumnType("integer");

                    b.Property<int>("ScheduledDayOfMonth")
                        .HasColumnType("integer");

                    b.Property<int>("ScheduledDayOfWeek")
                        .HasColumnType("integer");

                    b.Property<TimeOnly>("ScheduledTime")
                        .HasColumnType("time without time zone");

                    b.Property<bool>("ShouldSendFailEmail")
                        .HasColumnType("boolean");

                    b.Property<int>("Status")
                        .HasColumnType("integer");

                    b.Property<string>("ToAsset")
                        .HasColumnType("text");

                    b.Property<string>("WalletId")
                        .HasColumnType("text");

                    b.HasKey("LogId");

                    b.HasIndex("ClientId");

                    b.HasIndex("InstructionId");

                    b.ToTable("audit", "autoinvest");
                });

            modelBuilder.Entity("Service.AutoInvestManager.Domain.Models.InvestOrder", b =>
                {
                    b.Property<string>("Id")
                        .HasColumnType("text");

                    b.Property<string>("BrokerId")
                        .HasColumnType("text");

                    b.Property<string>("ClientId")
                        .HasColumnType("text");

                    b.Property<DateTime>("ExecutionTime")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("timestamp with time zone")
                        .HasDefaultValue(new DateTime(1, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc));

                    b.Property<decimal>("FromAmount")
                        .HasColumnType("numeric");

                    b.Property<string>("FromAsset")
                        .HasColumnType("text");

                    b.Property<string>("InvestInstructionId")
                        .HasColumnType("text");

                    b.Property<decimal>("Price")
                        .HasColumnType("numeric");

                    b.Property<int>("Status")
                        .HasColumnType("integer");

                    b.Property<decimal>("ToAmount")
                        .HasColumnType("numeric");

                    b.Property<string>("ToAsset")
                        .HasColumnType("text");

                    b.Property<string>("WalletId")
                        .HasColumnType("text");

                    b.HasKey("Id");

                    b.HasIndex("ClientId");

                    b.HasIndex("Status");

                    b.ToTable("orders", "autoinvest");
                });
#pragma warning restore 612, 618
        }
    }
}
