using MIC.Core.Interfaces;
using MIC.Infrastructure.Database;
using MIC.Infrastructure.Logging;
using MIC.MainApp.Forms;
using MIC.Services;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using System;
using System.IO;
using System.Windows.Forms;

namespace MIC.MainApp
{
    static class Program
    {
        /// <summary>
        /// 应用程序的主入口点。
        /// </summary>
        [STAThread]
        static void Main()
        {
            Application.EnableVisualStyles();
            Application.SetCompatibleTextRenderingDefault(false);

            try
            {
                Application.EnableVisualStyles();
                Application.SetCompatibleTextRenderingDefault(false);

                // 1. 创建 Host 并在其中注册所有服务
                var host = CreateHostBuilder().Build();

                // 2. 将 Host 交给 GlobalContext 方便全局访问
                GlobalContext.Initialize(host);

                // 3. 从 DI 容器中解析主窗体并运行
                var mainForm = host.Services.GetRequiredService<MainForm>();
                Application.Run(mainForm);
            }
            catch (Exception ex)
            {
                MessageBox.Show("启动严重错误: " + ex.Message);
            }
        }
    

    static IHostBuilder CreateHostBuilder() =>
        Host.CreateDefaultBuilder()
            .ConfigureServices((hostContext, services) =>
            {
                // 1. 先实例化一个临时的 Logger 和 Loader 用于扫描
                var tempLogger = new NLogLogger();
                var loader = new PluginLoader(tempLogger);
                var driverMap = loader.DiscoverDrivers(Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "Plugins"));

                // --- 基础基础设施 (Singleton) ---
                services.AddSingleton<ILoggerService, NLogLogger>();
                services.AddSingleton<SqliteDapperHelper>();

                //  注册驱动工厂 (注入发现的驱动映射表)
                services.AddSingleton<IDriverFactory>(sp => new DriverFactory(sp, driverMap));

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

                // --- 窗体也注册到 DI 中，这样窗体也可以通过构造函数接收服务 ---
                services.AddTransient<MainForm>();
                services.AddTransient<WorkflowEditorForm>();
            });

    } 
}