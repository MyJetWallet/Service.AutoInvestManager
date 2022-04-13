using System;
using System.Runtime.Serialization;
using Service.AutoInvestManager.Domain.Models;

namespace Service.AutoInvestManager.Grpc.Models;

[DataContract]
public class CreateInstructionResponse
{
    [DataMember(Order = 1)]
    public bool IsSuccess { get; set; }
        
    [DataMember(Order = 2)]
    public string ErrorMessage { get; set; }
        
    [DataMember(Order = 3)]
    public InvestInstruction Instruction { get; set; }
    
    [DataMember(Order = 4)]
    public DateTime NextExecutionDate { get; set; }
}