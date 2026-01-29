using System;
using System.IO;

namespace MIC.Core.Constants
{
    public static class GlobalConstants
    {
        // 基础路径
        public static readonly string BaseDirectory = AppDomain.CurrentDomain.BaseDirectory;

        // 插件路径
        public static readonly string PluginsPath = Path.Combine(BaseDirectory, "Plugins");

        // 配置文件路径
        public static readonly string ConfigPath = Path.Combine(BaseDirectory, "Configs");
        public static readonly string DeviceConfigPath = Path.Combine(ConfigPath, "devices.json");
        public static readonly string DashboardConfigPath = Path.Combine(ConfigPath, "dashboard.json");

        // 数据库名称
        public static readonly string DbName = "mic_data.db";
    }
}