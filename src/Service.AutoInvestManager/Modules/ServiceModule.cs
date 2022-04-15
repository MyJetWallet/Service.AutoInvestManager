using Autofac;
using Autofac.Core;
using Autofac.Core.Registration;
using MyJetWallet.Sdk.NoSql;
using MyJetWallet.Sdk.ServiceBus;
using Service.AutoInvestManager.Domain.Helpers;
using Service.AutoInvestManager.Domain.Models;
using Service.AutoInvestManager.Domain.Models.NoSql;
using Service.Liquidity.Converter.Client;

namespace Service.AutoInvestManager.Modules
{
    public class ServiceModule: Module
    {
        protected override void Load(ContainerBuilder builder)
        {
            builder.RegisterMyNoSqlWriter<InvestInstructionNoSqlEntity>(()=>Program.Settings.MyNoSqlWriterUrl, InvestInstructionNoSqlEntity.TableName);

            builder.RegisterLiquidityConverterClient(Program.Settings.LiquidityConverterGrpcServiceUrl);
            
            builder.RegisterLiquidityConverterManagerClient(Program.Settings.LiquidityConverterGrpcServiceUrl);
            
            var serviceBusClient = builder.RegisterMyServiceBusTcpClient(()=>Program.Settings.SpotServiceBusHostPort, Program.LogFactory);
            builder.RegisterMyServiceBusPublisher<InvestInstruction>(serviceBusClient, InvestInstruction.TopicName, true);

            builder
                .RegisterType<InstructionsRepository>()
                .AsSelf()
                .SingleInstance()
                .AutoActivate();
        }
    }
}