using System;
using System.Runtime.Serialization;

namespace Service.AutoInvestManager.Grpc.Models;

[DataContract]
public class GetOrdersRequest
{
    [DataMember(Order = 1)]
    public string SearchText { get; set; }
    [DataMember(Order = 2)] 
    public int Take { get; set; }
    [DataMember(Order = 3)]
    public DateTime LastSeen { get; set; }
}