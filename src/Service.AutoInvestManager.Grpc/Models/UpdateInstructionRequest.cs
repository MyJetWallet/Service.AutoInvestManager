using System;
using System.Runtime.Serialization;
using Service.AutoInvestManager.Domain.Models;

namespace Service.AutoInvestManager.Grpc.Models;

[DataContract]
public class UpdateInstructionRequest
{
    [DataMember(Order = 1)]public string ClientId { get; set; }
    [DataMember(Order = 2)]public string InstructionId { get; set; }
    [DataMember(Order = 3)]public decimal FromAmount { get; set; }
    [DataMember(Order = 4)]public string FromAsset { get; set; }
    
    [DataMember(Order = 5)]public ScheduleType ScheduleType { get; set; }
    [DataMember(Order = 8)] public DateTime ScheduledTime { get; set; }
    [DataMember(Order = 9)] public DayOfWeek ScheduledDayOfWeek { get; set; }
    [DataMember(Order = 10)] public int ScheduledDayOfMonth { get; set; }
}