using System;
using System.Runtime.Serialization;
using MyJetWallet.Domain;

namespace Service.AutoInvestManager.Domain.Models
{
    [DataContract]
    public class InvestInstruction
    {
        [DataMember(Order = 1)]public string Id { get; set; }
        
        [DataMember(Order = 2)]public string ClientId { get; set; }
        [DataMember(Order = 3)]public string BrokerId { get; set; }
        [DataMember(Order = 4)]public string WalletId { get; set; }
        
        [DataMember(Order = 5)]public decimal FromAmount { get; set; }
        [DataMember(Order = 6)]public string FromAsset { get; set; }
        [DataMember(Order = 7)]public string ToAsset { get; set; }
        
        [DataMember(Order = 8)]public InstructionStatus Status { get; set; }
        
        [DataMember(Order = 9)]public ScheduleType ScheduleType { get; set; }
        [DataMember(Order = 10)]public TimeOnly ScheduledTime { get; set; }
        [DataMember(Order = 11)]public DayOfWeek ScheduledDayOfWeek { get; set; }
        [DataMember(Order = 12)]public int ScheduledDayOfMonth { get; set; }
        
        [DataMember(Order = 13)]public DateTime CreationTime { get; set; }
        [DataMember(Order = 14)]public DateTime LastExecutionTime { get; set; }
        [DataMember(Order = 15)]public bool ShouldSendFailEmail { get; set; }
    }
    
}