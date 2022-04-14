using System;
using System.Runtime.Serialization;
using MyJetWallet.Domain;

namespace Service.AutoInvestManager.Domain.Models
{
    [DataContract]
    public class InvestOrder
    {
        public const string TopicName = "recurringbuy-investorders";
        
        [DataMember(Order = 1)]public string Id { get; set; }
        [DataMember(Order = 2)]public string ClientId { get; set; }
        [DataMember(Order = 3)]public string BrokerId { get; set; }
        [DataMember(Order = 4)]public string WalletId { get; set; }
        [DataMember(Order = 5)]public string InvestInstructionId { get; set; }
        
        [DataMember(Order = 6)]public decimal FromAmount { get; set; }
        [DataMember(Order = 7)]public string FromAsset { get; set; }
        [DataMember(Order = 8)]public decimal ToAmount { get; set; }
        [DataMember(Order = 9)]public string ToAsset { get; set; }
        [DataMember(Order = 10)] public decimal Price { get; set; }
        
        [DataMember(Order = 11)] public OrderStatus Status { get; set; }
        [DataMember(Order = 12)] public DateTime ExecutionTime { get; set; }
        
        [DataMember(Order = 13)]public string ErrorText { get; set; }
        [DataMember(Order = 14)]public string QuoteId { get; set; }

    }
}