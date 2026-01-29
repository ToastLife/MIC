namespace MIC.Models.DTOs
{
    public class DeviceConfigDto
    {
        public string DeviceId { get; set; }
        public string PluginDll { get; set; } // DLL 文件名
        public string DriverClass { get; set; } // 完整的类名(含命名空间)
        public object DriverConfig { get; set; } // 传给驱动 Initialize 方法的参数
    }
}