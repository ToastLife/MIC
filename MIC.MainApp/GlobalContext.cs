using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using MIC.Core.Interfaces;
using MIC.Infrastructure.Database;
using MIC.Services;
using System;

namespace MIC.MainApp
{
    public static class GlobalContext
    {
        // 全局服务提供者
        public static IServiceProvider ServiceProvider { get; private set; }
        private static IHost _host;

        public static void Initialize(IHost host)
        {
            _host = host;
            ServiceProvider = host.Services;
        }

        // 快捷访问方法 (保留之前的调用习惯，但底层走 DI)
        public static T GetService<T>() => ServiceProvider.GetRequiredService<T>();

        // 映射原有属性
        public static ILoggerService Logger => GetService<ILoggerService>();
        public static SqliteDapperHelper DbHelper => GetService<SqliteDapperHelper>();
        public static ProjectManager ProjectManager => GetService<ProjectManager>();
        public static DeviceManager DeviceManager => GetService<DeviceManager>();
        public static WorkflowEngine WorkflowEngine => GetService<WorkflowEngine>();
        public static PollingService PollingService => GetService<PollingService>();
        public static PluginLoader PluginLoader => GetService<PluginLoader>();
        public static AuthService AuthService => GetService<AuthService>();

        /// <summary>
        /// 程序退出时释放资源
        /// </summary>
        public static void Shutdown()
        {
            PollingService?.Stop();
            WorkflowEngine?.Stop();
            DeviceManager?.DisconnectAll();
            Logger?.Info("系统已安全关闭");
        }
    }
}