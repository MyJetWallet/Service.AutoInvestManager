using System.Diagnostics;
using Microsoft.EntityFrameworkCore;
using MyJetWallet.Sdk.Postgres;
using MyJetWallet.Sdk.Service;
using Service.AutoInvestManager.Domain.Models;

namespace Service.AutoInvestManager.Postgres
{
    public class DatabaseContext : MyDbContext
    {
        public const string Schema = "autoinvest";

        private const string InstructionsTableName = "instructions";
        private const string OrdersTableName = "orders";
        private const string InstructionsAuditTableName = "audit";


        private Activity _activity;

        public DatabaseContext(DbContextOptions options) : base(options)
        {
        }

        public DbSet<InvestInstruction> Instructions { get; set; }
        public DbSet<InvestOrder> Orders { get; set; }
        public DbSet<InvestInstructionAuditRecord> AuditRecords { get; set; }

        public static DatabaseContext Create(DbContextOptionsBuilder<DatabaseContext> options)
        {
            var activity = MyTelemetry.StartActivity($"Database context {Schema}")?.AddTag("db-schema", Schema);

            var ctx = new DatabaseContext(options.Options) { _activity = activity };

            return ctx;
        }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            modelBuilder.HasDefaultSchema(Schema);

            SetInstructionsEntry(modelBuilder);
            SetOrderEntry(modelBuilder);
            SetAuditEntry(modelBuilder);

            base.OnModelCreating(modelBuilder);
        }

        private void SetInstructionsEntry(ModelBuilder modelBuilder)
        {
            modelBuilder.Entity<InvestInstruction>().ToTable(InstructionsTableName);
            modelBuilder.Entity<InvestInstruction>().HasKey(e => e.Id);
            modelBuilder.Entity<InvestInstruction>().Property(e => e.ClientId);
            modelBuilder.Entity<InvestInstruction>().Property(e => e.BrokerId);
            modelBuilder.Entity<InvestInstruction>().Property(e => e.WalletId);
            modelBuilder.Entity<InvestInstruction>().Property(e => e.FromAmount);
            modelBuilder.Entity<InvestInstruction>().Property(e => e.FromAsset);
            modelBuilder.Entity<InvestInstruction>().Property(e => e.ToAsset);
            modelBuilder.Entity<InvestInstruction>().Property(e => e.Status);
            modelBuilder.Entity<InvestInstruction>().Property(e => e.ScheduleType);
            modelBuilder.Entity<InvestInstruction>().Property(e => e.ScheduledTime);
            modelBuilder.Entity<InvestInstruction>().Property(e => e.ScheduledDayOfWeek);
            modelBuilder.Entity<InvestInstruction>().Property(e => e.ScheduledDayOfMonth);
            modelBuilder.Entity<InvestInstruction>().Property(e => e.ScheduleType);
            modelBuilder.Entity<InvestInstruction>().Property(e => e.CreationTime).HasDefaultValue(DateTime.MinValue);
            modelBuilder.Entity<InvestInstruction>().Property(e => e.LastExecutionTime).HasDefaultValue(DateTime.MinValue);
            modelBuilder.Entity<InvestInstruction>().Property(e => e.ShouldSendFailEmail);

            modelBuilder.Entity<InvestInstruction>().HasIndex(e => e.ClientId);
            modelBuilder.Entity<InvestInstruction>().HasIndex(e => e.Status);
        }

        private void SetOrderEntry(ModelBuilder modelBuilder)
        {
            modelBuilder.Entity<InvestOrder>().ToTable(OrdersTableName);
            modelBuilder.Entity<InvestOrder>().HasKey(e => e.Id);
            modelBuilder.Entity<InvestOrder>().Property(e => e.InvestInstructionId);
            modelBuilder.Entity<InvestOrder>().Property(e => e.ClientId);
            modelBuilder.Entity<InvestOrder>().Property(e => e.BrokerId);
            modelBuilder.Entity<InvestOrder>().Property(e => e.WalletId);
            modelBuilder.Entity<InvestOrder>().Property(e => e.FromAmount);
            modelBuilder.Entity<InvestOrder>().Property(e => e.FromAsset);
            modelBuilder.Entity<InvestOrder>().Property(e => e.ToAsset);
            modelBuilder.Entity<InvestOrder>().Property(e => e.ToAmount);
            modelBuilder.Entity<InvestOrder>().Property(e => e.Status);
            modelBuilder.Entity<InvestOrder>().Property(e => e.Price);
            modelBuilder.Entity<InvestOrder>().Property(e => e.ExecutionTime).HasDefaultValue(DateTime.MinValue);
            
            modelBuilder.Entity<InvestOrder>().HasIndex(e => e.ClientId);
            modelBuilder.Entity<InvestOrder>().HasIndex(e => e.Status);

        }
        
        private void SetAuditEntry(ModelBuilder modelBuilder)
        {
            modelBuilder.Entity<InvestInstructionAuditRecord>().ToTable(InstructionsAuditTableName);
            modelBuilder.Entity<InvestInstructionAuditRecord>().HasKey(e => e.LogId);
            modelBuilder.Entity<InvestInstructionAuditRecord>().Property(e => e.LogId).UseIdentityColumn();

            modelBuilder.Entity<InvestInstructionAuditRecord>().Property(e => e.InstructionId);
            modelBuilder.Entity<InvestInstructionAuditRecord>().Property(e => e.ClientId);
            modelBuilder.Entity<InvestInstructionAuditRecord>().Property(e => e.BrokerId);
            modelBuilder.Entity<InvestInstructionAuditRecord>().Property(e => e.WalletId);
            modelBuilder.Entity<InvestInstructionAuditRecord>().Property(e => e.FromAmount);
            modelBuilder.Entity<InvestInstructionAuditRecord>().Property(e => e.FromAsset);
            modelBuilder.Entity<InvestInstructionAuditRecord>().Property(e => e.ToAsset);
            modelBuilder.Entity<InvestInstructionAuditRecord>().Property(e => e.Status);
            modelBuilder.Entity<InvestInstructionAuditRecord>().Property(e => e.ScheduleType);
            modelBuilder.Entity<InvestInstructionAuditRecord>().Property(e => e.ScheduledTime);
            modelBuilder.Entity<InvestInstructionAuditRecord>().Property(e => e.ScheduledDayOfWeek);
            modelBuilder.Entity<InvestInstructionAuditRecord>().Property(e => e.ScheduledDayOfMonth);
            modelBuilder.Entity<InvestInstructionAuditRecord>().Property(e => e.ScheduleType);
            modelBuilder.Entity<InvestInstructionAuditRecord>().Property(e => e.CreationTime).HasDefaultValue(DateTime.MinValue);
            modelBuilder.Entity<InvestInstructionAuditRecord>().Property(e => e.LastExecutionTime).HasDefaultValue(DateTime.MinValue);
            modelBuilder.Entity<InvestInstructionAuditRecord>().Property(e => e.ShouldSendFailEmail);
            modelBuilder.Entity<InvestInstructionAuditRecord>().Property(e => e.LogTimestamp).HasDefaultValue(DateTime.MinValue);

            modelBuilder.Entity<InvestInstructionAuditRecord>().HasIndex(e => e.ClientId);
            modelBuilder.Entity<InvestInstructionAuditRecord>().HasIndex(e => e.InstructionId);

        }
        
        public async Task<int> UpsertAsync(IEnumerable<InvestInstruction> entities)
        {
            var result = await Instructions.UpsertRange(entities).AllowIdentityMatch().RunAsync();
            return result;
        }
        
        public async Task<int> UpsertAsync(IEnumerable<InvestOrder> entities)
        {
            var result = await Orders.UpsertRange(entities).AllowIdentityMatch().RunAsync();
            return result;
        }

    }
}