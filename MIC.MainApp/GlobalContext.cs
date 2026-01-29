using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using MIC.Core.Interfaces;
using MIC.Infrastructure.Database;
using MIC.Services;
using System;

namespace MIC.MainApp
{
    /// <summary>
    /// 全局上下文类。提供对应用程序中所有核心服务的访问，避免到处传递依赖项。
    /// 通过 DI 容器获取服务，保持测试友好性
    /// </summary>
    public static class GlobalContext
    {
        /// <summary>
        /// 全局服务提供者（DI 容器）
        /// </summary>
        public static IServiceProvider ServiceProvider { get; private set; }
        private static IHost _host;

        /// <summary>
        /// 初始化全局上下文。应在 Program.Main 中调用一次
        /// </summary>
        /// <param name="host">配置好的 Host 实例</param>
        public static void Initialize(IHost host)
        {
            _host = host;
            ServiceProvider = host.Services;
        }

        /// <summary>
        /// 从 DI 容器中获取指定类型的服务（通用方法）
        /// </summary>
        /// <typeparam name="T">服务接口类型</typeparam>
        /// <returns>服务实例</returns>
        public static T GetService<T>() => ServiceProvider.GetRequiredService<T>();

        /// <summary>
        /// 获取日志服务
        /// </summary>
        public static ILoggerService Logger => GetService<ILoggerService>();
        
        /// <summary>
        /// 获取数据库辅助类（SQLite Dapper 封装）
        /// </summary>
        public static SqliteDapperHelper DbHelper => GetService<SqliteDapperHelper>();
        
        /// <summary>
        /// 获取项目/方案管理服务
        /// </summary>
        public static ProjectManager ProjectManager => GetService<ProjectManager>();
        
        /// <summary>
        /// 获取设备管理服务
        /// </summary>
        public static DeviceManager DeviceManager => GetService<DeviceManager>();
        
        /// <summary>
        /// 获取工作流引擎服务
        /// </summary>
        public static WorkflowEngine WorkflowEngine => GetService<WorkflowEngine>();
        
        /// <summary>
        /// 获取轮询服务
        /// </summary>
        public static PollingService PollingService => GetService<PollingService>();
        
        /// <summary>
        /// 获取插件加载器服务
        /// </summary>
        public static PluginLoader PluginLoader => GetService<PluginLoader>();
        
        /// <summary>
        /// 获取认证服务
        /// </summary>
        public static AuthService AuthService => GetService<AuthService>();

        /// <summary>
        /// 程序退出时释放资源。停止所有后台任务，断开设备连接，记录日志
        /// </summary>
        public static void Shutdown()
        {
            // 不再需要手动调用 Stop 方法，IHostedService 会自动管理生命周期
            DeviceManager?.DisconnectAll();
            Logger?.Info("系统已安全关闭");
        }
    }
}