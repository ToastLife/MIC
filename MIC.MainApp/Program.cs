using DryIoc.Microsoft.DependencyInjection;
using MIC.Core.Interfaces;
using MIC.Infrastructure.Database;
using MIC.Infrastructure.Logging;
using MIC.Services;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging; // 必须引用：标准日志接口
using NLog.Extensions.Logging;      // 必须引用：NLog 适配器
using System;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Windows.Forms;

namespace MIC.MainApp
{
    static class Program
    {
        /// <summary>
        /// 应用程序的主入口点。初始化 WinForms 应用程序，创建 DI 容器，启动主窗体
        /// </summary>
        [STAThread]
        static void Main()
        {
            Application.EnableVisualStyles();
            Application.SetCompatibleTextRenderingDefault(false);

            try
            {
                // 1. 创建 Host 并在其中注册所有服务
                var host = CreateHostBuilder().Build();

                // 2. 将 Host 交给 GlobalContext 方便全局访问
                GlobalContext.Initialize(host);

                // 3. 启动后台服务
                host.Start();

                // 4. 从 DI 容器中解析主窗体并运行
                var mainForm = host.Services.GetRequiredService<MainForm>();
                Application.Run(mainForm);

                // UI 退出后停机
                host.StopAsync().GetAwaiter().GetResult();
                host.Dispose();
            }
            catch (Exception ex)
            {
                MessageBox.Show("启动严重错误: " + ex.Message);
            }
        }

        /// <summary>
        /// 创建并配置 DI 容器。注册所有应用程序需要的服务（日志、数据库、业务服务等）
        /// </summary>
        static IHostBuilder CreateHostBuilder() =>
        Host.CreateDefaultBuilder()
            .ConfigureAppConfiguration((context, config) =>
            {
                config.AddJsonFile("appsettings.json", optional: false, reloadOnChange: true);
            })
            .UseServiceProviderFactory(new DryIocServiceProviderFactory())
            .ConfigureLogging((hostContext, logging) =>
                {
                    // 1. 清除默认提供程序（也就是移除默认的 Console, Debug 等）
                    logging.ClearProviders();

                    // 2. 设置最小日志级别（这里设为 Trace，具体由 NLog.config 接管控制）
                    logging.SetMinimumLevel(LogLevel.Trace);

                    // 3. 注入 NLog
                    // 这行代码会将 NLog 绑定到 ILogger<T> 接口上
                    logging.AddNLog();
                })
            .ConfigureServices((hostContext, services) =>
            {
                var configuration = hostContext.Configuration;

                // 注册配置
                services.AddSingleton(configuration);

                // 注册 HostedService
                services.AddHostedService<PollingService>();
                services.AddHostedService<WorkflowEngine>();

                // --- 基础基础设施 (Singleton) ---
                services.AddSingleton<ILoggerService, NLogLogger>();
                services.AddSingleton<SqliteDapperHelper>();

                // --- 权限管理 ---
                services.AddSingleton<AuthService>();

                // --- 业务逻辑服务 (Singleton) ---
                services.AddSingleton<DeviceManager>();
                services.AddSingleton<ProjectManager>();
                services.AddSingleton<PollingService>();

                // --- 逻辑执行引擎, 每次运行流程可以考虑建立一个独立范围
                services.AddScoped<WorkflowEngine>();

                // --- 插件管理器 (Singleton) ---
                services.AddSingleton<PluginLoader>();

                // --- 主窗体 (Singleton) ---
                services.AddSingleton<MainForm>();

                // 动态加载插件并注册
                RegisterPlugins(services, configuration);
            });

        /// <summary>
        /// 动态加载插件并注册到 DI 容器
        /// </summary>
        /// <param name="services">服务集合</param>
        /// <param name="configuration">应用程序配置</param>
        private static void RegisterPlugins(IServiceCollection services, IConfiguration configuration)
        {
            var pluginPath = configuration["Plugins:Path"];
            if (!Directory.Exists(pluginPath)) return;

            foreach (var dll in Directory.GetFiles(pluginPath, "*.dll"))
            {
                try
                {
                    var assembly = Assembly.LoadFrom(dll);
                    var types = assembly.GetTypes().Where(t => typeof(IDeviceDriver).IsAssignableFrom(t) && !t.IsInterface);
                    foreach (var type in types)
                    {
                        services.AddTransient(type);
                    }
                }
                catch (Exception ex)
                {
                    Console.WriteLine($"Failed to load plugin {dll}: {ex.Message}");
                }
            }
        }
    }
}