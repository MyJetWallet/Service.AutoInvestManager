using Autofac;
using Autofac.Core;
using Autofac.Core.Registration;
using MyJetWallet.Sdk.NoSql;
using Service.AutoInvestManager.Domain.Models.NoSql;
using Service.AutoInvestManager.Helpers;
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
            
            builder
                .RegisterType<InstructionsRepository>()
                .AsSelf()
                .SingleInstance()
                .AutoActivate();
        }
    }
}