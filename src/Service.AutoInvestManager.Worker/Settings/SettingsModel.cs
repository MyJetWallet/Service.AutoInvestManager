using MyJetWallet.Sdk.Service;
using MyYamlParser;

namespace Service.AutoInvestManager.Worker.Settings
{
    public class SettingsModel
    {
        [YamlProperty("AutoInvestManager.SeqServiceUrl")]
        public string SeqServiceUrl { get; set; }

        [YamlProperty("AutoInvestManager.ZipkinUrl")]
        public string ZipkinUrl { get; set; }

        [YamlProperty("AutoInvestManager.ElkLogs")]
        public LogElkSettings ElkLogs { get; set; }
        
        [YamlProperty("AutoInvestManager.MyNoSqlReaderHostPort")]
        public string MyNoSqlReaderHostPort { get; set; }

        [YamlProperty("AutoInvestManager.MyNoSqlWriterUrl")]
        public string MyNoSqlWriterUrl { get; set; }
        
        [YamlProperty("AutoInvestManager.SpotServiceBusHostPort")]
        public string SpotServiceBusHostPort { get; set; }
        
        [YamlProperty("AutoInvestManager.PostgresConnectionString")]
        public string PostgresConnectionString { get; set; }
        
        [YamlProperty("AutoInvestManager.LiquidityConverterGrpcServiceUrl")]
        public string LiquidityConverterGrpcServiceUrl { get; set; }
        
        [YamlProperty("AutoInvestManager.TimerPeriodInSeconds")]
        public int TimerPeriodInSeconds { get; set; }
        
        [YamlProperty("AutoInvestManager.EmailSenderGrpcServiceUrl")]
        public string EmailSenderGrpcServiceUrl { get; set; }
    }
}
