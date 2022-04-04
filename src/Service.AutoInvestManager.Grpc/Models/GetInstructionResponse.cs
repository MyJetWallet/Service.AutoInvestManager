using System;
using System.Collections.Generic;
using System.Runtime.Serialization;
using Service.AutoInvestManager.Domain.Models;

namespace Service.AutoInvestManager.Grpc.Models;

[DataContract]
public class GetInstructionResponse
{
    [DataMember(Order = 1)] public List<InvestInstruction> Instructions { get; set; }
}