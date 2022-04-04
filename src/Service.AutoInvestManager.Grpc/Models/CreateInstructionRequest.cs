using System;
using System.Runtime.Serialization;
using Service.AutoInvestManager.Domain.Models;

namespace Service.AutoInvestManager.Grpc.Models
{
    [DataContract] 
    public class CreateInstructionRequest
    {
        [DataMember(Order = 1)] public string ClientId { get; set; }
        [DataMember(Order = 2)] public string BrokerId { get; set; }
        [DataMember(Order = 3)] public string WalletId { get; set; }
        
        [DataMember(Order = 4)] public decimal FromAmount { get; set; }
        [DataMember(Order = 5)] public string FromAsset { get; set; }
        [DataMember(Order = 6)] public string ToAsset { get; set; }
        
        [DataMember(Order = 7)] public ScheduleType ScheduleType { get; set; }
        [DataMember(Order = 8)] public DateTime ScheduledTime { get; set; }
        [DataMember(Order = 9)] public DayOfWeek ScheduledDayOfWeek { get; set; }
        [DataMember(Order = 10)] public int ScheduledDayOfMonth { get; set; }
        
        [DataMember(Order = 11)] public bool ExecuteImmediately { get; set; }

    }
}