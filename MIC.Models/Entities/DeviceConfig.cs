namespace MIC.Models.Entities
{
    // 用于序列化保存到JSON的配置模型
    public class DeviceConfig
    {
        public string DeviceId { get; set; }
        public string PluginType { get; set; } // 例如 "ModbusTcp"
        public string ConnectionString { get; set; } // IP=127.0.0.1;Port=502
    }
}