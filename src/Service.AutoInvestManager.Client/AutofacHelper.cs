using Autofac;
using Service.AutoInvestManager.Grpc;

// ReSharper disable UnusedMember.Global

namespace Service.AutoInvestManager.Client
{
    public static class AutofacHelper
    {
        public static void RegisterAutoInvestManagerClient(this ContainerBuilder builder, string grpcServiceUrl)
        {
            var factory = new AutoInvestManagerClientFactory(grpcServiceUrl);

            builder.RegisterInstance(factory.GetHelloService()).As<IAutoInvestService>().SingleInstance();
        }
    }
}
