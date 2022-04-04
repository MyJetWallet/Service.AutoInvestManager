using MyJetWallet.Sdk.Service;
using MyYamlParser;

namespace Service.AutoInvestManager.Settings
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
        
        // [YamlProperty("AutoInvestManager.TimerPeriodInSeconds")]
        // public int TimerPeriodInSeconds { get; set; }
    }
}
