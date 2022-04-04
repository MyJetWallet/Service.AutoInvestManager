using System;
using System.Runtime.Serialization;
using Service.AutoInvestManager.Domain.Models;

namespace Service.AutoInvestManager.Grpc.Models
{
    [DataContract]
    public class SwitchInstructionRequest
    {
        [DataMember(Order = 1)]public string ClientId { get; set; }
        [DataMember(Order = 2)]public string InstructionId { get; set; }
        [DataMember(Order = 3)]public InstructionStatus Status { get; set; }
    }
}