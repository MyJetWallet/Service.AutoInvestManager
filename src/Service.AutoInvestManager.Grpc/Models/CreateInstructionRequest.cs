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
        [DataMember(Order = 7)] public ScheduleType ScheduleType { get; set; }
        [DataMember(Order = 12)] public string QuoteId { get; set; }


    }
}