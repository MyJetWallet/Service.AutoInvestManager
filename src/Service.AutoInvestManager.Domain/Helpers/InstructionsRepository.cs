using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using MyNoSqlServer.Abstractions;
using Service.AutoInvestManager.Domain.Models;
using Service.AutoInvestManager.Domain.Models.NoSql;
using Service.AutoInvestManager.Postgres;

namespace Service.AutoInvestManager.Domain.Helpers
{
    public class InstructionsRepository
    {
        private readonly DbContextOptionsBuilder<DatabaseContext> _dbContextOptionsBuilder;
        private readonly IMyNoSqlServerDataWriter<InvestInstructionNoSqlEntity> _writer;
        private readonly ILogger<InstructionsRepository> _logger;
        
        public InstructionsRepository(
            DbContextOptionsBuilder<DatabaseContext> dbContextOptionsBuilder, 
            IMyNoSqlServerDataWriter<InvestInstructionNoSqlEntity> writer, 
            ILogger<InstructionsRepository> logger)
        {
            _dbContextOptionsBuilder = dbContextOptionsBuilder;
            _writer = writer;
            _logger = logger;
        }

        public async Task UpsertInstructions(InvestInstruction instruction)
        {
            await UpsertInstructions(new List<InvestInstruction> {instruction});
        }

        public async Task UpsertInstructions(List<InvestInstruction> instructions)
        {
            await using var context = new DatabaseContext(_dbContextOptionsBuilder.Options);
            await context.UpsertAsync(instructions);
            await _writer.BulkInsertOrReplaceAsync(instructions.Where(t=>t.Status != InstructionStatus.Deleted).Select(InvestInstructionNoSqlEntity.Create));
            foreach (var instruction in instructions.Where(t=>t.Status == InstructionStatus.Deleted))
            {
                await _writer.DeleteAsync(InvestInstructionNoSqlEntity.GeneratePartitionKey(instruction.ClientId),InvestInstructionNoSqlEntity.GenerateRowKey(instruction.Id));
            }
        }
        
        public async Task UpsertOrders(InvestOrder order)
        {
            await UpsertOrders(new List<InvestOrder> { order });
        }

        public async Task UpsertOrders(List<InvestOrder> orders)
        {
            await using var context = new DatabaseContext(_dbContextOptionsBuilder.Options);
            await context.UpsertAsync(orders);
        }

        public async Task UpsertInstructionAudit(InvestInstruction instruction)
        {
            await using var context = new DatabaseContext(_dbContextOptionsBuilder.Options);
            await context.AuditRecords.AddAsync(InvestInstructionAuditRecord.Create(instruction));
            await context.SaveChangesAsync();
        }

        public async Task<List<InvestInstruction>> GetInvestInstructions(string searchText, int take, DateTime lastSeen, InstructionStatus? status)
        {
            try
            {
                await using var context = new DatabaseContext(_dbContextOptionsBuilder.Options);
                if (take == 0)
                    take = 20;

                var query = context.Instructions.AsQueryable();
                if (lastSeen != DateTime.MinValue)
                    query = query.Where(t => t.CreationTime < lastSeen);
                
                if (status != null)
                    query = query.Where(t => t.Status == status);

                if (!string.IsNullOrWhiteSpace(searchText))
                    query = query.Where(t => t.ClientId.Contains(searchText) ||
                                             t.WalletId.Contains(searchText) ||
                                             t.FromAsset.Contains(searchText) ||
                                             t.ToAsset.Contains(searchText)||
                                             t.Id.Contains(searchText));

                return await query.OrderByDescending(t => t.CreationTime).Take(take).ToListAsync();
            }
            catch (Exception e)
            {
                _logger.LogError(e, "When getting invest instructions");
                throw;
            }
        }

        public async Task<List<InvestInstruction>> GetInstructionsByStatus(InstructionStatus status)
        {
            await using var context = new DatabaseContext(_dbContextOptionsBuilder.Options);
            return await context.Instructions.Where(t=>t.Status == status).ToListAsync();
        }

        public async Task<List<InvestOrder>> GetScheduledInvestOrders()
        {
            await using var context = new DatabaseContext(_dbContextOptionsBuilder.Options);
            return await context.Orders.Where(t=>t.Status == OrderStatus.Scheduled).ToListAsync();
        }
        
        public async Task<InvestInstruction> GetInstructionById(string id)
        {
            await using var context = new DatabaseContext(_dbContextOptionsBuilder.Options);
            return await context.Instructions.FirstOrDefaultAsync(t=>t.Id == id);
        }
        
        public async Task<List<InvestOrder>> GetInvestOrders(string searchText, int take, DateTime lastSeen)
        {
            try
            {
                await using var context = new DatabaseContext(_dbContextOptionsBuilder.Options);
                if (take == 0)
                    take = 20;

                var query = context.Orders.AsQueryable();
                if (lastSeen != DateTime.MinValue)
                    query = query.Where(t => t.CreationTime < lastSeen);

                if (!string.IsNullOrWhiteSpace(searchText))
                    query = query.Where(t => t.ClientId.Contains(searchText) ||
                                             t.WalletId.Contains(searchText) ||
                                             t.FromAsset.Contains(searchText) ||
                                             t.ToAsset.Contains(searchText)||
                                             t.InvestInstructionId.Equals(searchText) ||
                                             t.Id.Equals(searchText));
                
                return await query.OrderByDescending(t => t.ExecutionTime).Take(take).ToListAsync();
            }
            catch (Exception e)
            {
                _logger.LogError(e, "When getting invest orders");
                throw;
            }
        }
        
        public async Task<InvestInstruction> GetInvestInstructionById(string id)
        {
            try
            {
                await using var context = new DatabaseContext(_dbContextOptionsBuilder.Options);
                return await context.Instructions.FirstOrDefaultAsync(t=>t.Id == id);
            }
            catch (Exception e)
            {
                _logger.LogError(e, "When getting invest instructions");
                throw;
            }
        }
    }
}