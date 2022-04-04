using System;
using System.Collections.Generic;
using System.Runtime.Serialization;
using Service.AutoInvestManager.Domain.Models;

namespace Service.AutoInvestManager.Grpc.Models;

[DataContract]
public class GetOrdersResponse
{
    [DataMember(Order = 1)]
    public List<InvestOrder> Orders { get; set; }
}