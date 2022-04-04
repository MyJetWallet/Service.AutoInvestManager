using System;
using System.Runtime.Serialization;
using Service.AutoInvestManager.Domain.Models;

namespace Service.AutoInvestManager.Grpc.Models
{
    [DataContract]
    public class RemoveInstructionRequest
    {
        [DataMember(Order = 1)]public string ClientId { get; set; }
        [DataMember(Order = 2)]public string InstructionId { get; set; }
    }
}