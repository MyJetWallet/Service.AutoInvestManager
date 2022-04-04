using System.ServiceModel;
using System.Threading.Tasks;
using Service.AutoInvestManager.Grpc.Models;

namespace Service.AutoInvestManager.Grpc
{
    [ServiceContract]
    public interface IAutoInvestService
    {
        [OperationContract]
        Task<OperationResponse> CreateInstruction(CreateInstructionRequest request);
        
        [OperationContract]
        Task<OperationResponse> UpdateInstruction(UpdateInstructionRequest request);
        
        [OperationContract]
        Task<OperationResponse> RemoveInstruction(RemoveInstructionRequest request);
        
        [OperationContract]
        Task<OperationResponse> SwitchInstruction(SwitchInstructionRequest request);
        
        [OperationContract]
        Task<GetOrdersResponse> GetOrders(GetOrdersRequest request);
        
        [OperationContract]
        Task<GetInstructionResponse> GetInstructions(GetInstructionRequest request);
    }
}