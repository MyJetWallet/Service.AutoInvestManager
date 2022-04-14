using Autofac;
using MyJetWallet.Sdk.NoSql;
using MyJetWallet.Sdk.ServiceBus;
using Service.AutoInvestManager.Domain.Helpers;
using Service.AutoInvestManager.Domain.Models;
using Service.AutoInvestManager.Domain.Models.NoSql;
using Service.AutoInvestManager.Worker.Jobs;
using Service.EmailSender.Client;
using Service.Liquidity.Converter.Client;

namespace Service.AutoInvestManager.Worker.Modules
{
    public class ServiceModule: Module
    {
        protected override void Load(ContainerBuilder builder)
        {
            builder.RegisterMyNoSqlWriter<InvestInstructionNoSqlEntity>(()=>Program.Settings.MyNoSqlWriterUrl, InvestInstructionNoSqlEntity.TableName);
            builder.RegisterLiquidityConverterClient(Program.Settings.LiquidityConverterGrpcServiceUrl); 
            builder.RegisterEmailSenderClient(Program.Settings.EmailSenderGrpcServiceUrl);

            var serviceBusClient = builder.RegisterMyServiceBusTcpClient(()=>Program.Settings.SpotServiceBusHostPort, Program.LogFactory);
            builder.RegisterMyServiceBusPublisher<InvestOrder>(serviceBusClient, InvestOrder.TopicName, true);
            
            builder
                .RegisterType<InstructionsRepository>()
                .AsSelf()
                .SingleInstance()
                .AutoActivate();
            
            builder
                .RegisterType<OrdersProcessingJob>()
                .AsSelf()
                .SingleInstance()
                .AutoActivate();
        }
    }
}