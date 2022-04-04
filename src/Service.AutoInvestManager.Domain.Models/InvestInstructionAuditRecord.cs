using System;
using System.Runtime.Serialization;
using MyJetWallet.Domain;

namespace Service.AutoInvestManager.Domain.Models
{
    public class InvestInstructionAuditRecord
    {
        public long LogId { get; set; }
        public DateTime LogTimestamp { get; set; }
        public string InstructionId { get; set; }
        public string ClientId { get; set; }
        public string BrokerId { get; set; }
        public string WalletId { get; set; }
        public decimal FromAmount { get; set; }
        public string FromAsset { get; set; }
        public string ToAsset { get; set; }
        public InstructionStatus Status { get; set; }
        public ScheduleType ScheduleType { get; set; }
        public TimeOnly ScheduledTime { get; set; }
        public DayOfWeek ScheduledDayOfWeek { get; set; }
        public int ScheduledDayOfMonth { get; set; }
        public DateTime CreationTime { get; set; }
        public DateTime LastExecutionTime { get; set; }
        public bool ShouldSendFailEmail { get; set; }

        public static InvestInstructionAuditRecord Create(InvestInstruction instruction)
        {
            return new InvestInstructionAuditRecord
            {
                InstructionId = instruction.Id,
                ClientId = instruction.ClientId,
                BrokerId = instruction.BrokerId,
                WalletId = instruction.WalletId,
                FromAmount = instruction.FromAmount,
                FromAsset = instruction.FromAsset,
                ToAsset = instruction.ToAsset,
                Status = instruction.Status,
                ScheduleType = instruction.ScheduleType,
                ScheduledTime = instruction.ScheduledTime,
                ScheduledDayOfWeek = instruction.ScheduledDayOfWeek,
                ScheduledDayOfMonth = instruction.ScheduledDayOfMonth,
                CreationTime = instruction.CreationTime,
                LastExecutionTime = instruction.LastExecutionTime,
                ShouldSendFailEmail = instruction.ShouldSendFailEmail,
                LogTimestamp = DateTime.UtcNow
            };
        }
    }
}