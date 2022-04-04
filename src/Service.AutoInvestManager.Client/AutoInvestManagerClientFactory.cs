using JetBrains.Annotations;
using MyJetWallet.Sdk.Grpc;
using Service.AutoInvestManager.Grpc;

namespace Service.AutoInvestManager.Client
{
    [UsedImplicitly]
    public class AutoInvestManagerClientFactory: MyGrpcClientFactory
    {
        public AutoInvestManagerClientFactory(string grpcServiceUrl) : base(grpcServiceUrl)
        {
        }

        public IAutoInvestService GetHelloService() => CreateGrpcService<IAutoInvestService>();
    }
}
