using MIC.Core.Interfaces;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Options;
using System.Threading;
using System.Threading.Tasks;

namespace MIC.Services
{
    /// <summary>
    /// 轮询服务，继承 BackgroundService。
    /// </summary>
    public class PollingService : BackgroundService
    {
        private readonly DeviceManager _deviceManager;
        private readonly ILoggerService _logger;
        private readonly PollingServiceOptions _options;

        public PollingService(DeviceManager deviceManager, ILoggerService logger, IOptions<PollingServiceOptions> options)
        {
            _deviceManager = deviceManager;
            _logger = logger;
            _options = options.Value;
        }

        protected override async Task ExecuteAsync(CancellationToken stoppingToken)
        {
            _logger.Info("轮询服务启动");

            while (!stoppingToken.IsCancellationRequested)
            {
                foreach (var device in _deviceManager.GetAllDevices())
                {
                    if (!device.IsConnected)
                    {
                        await Task.Run(() => device.Connect(), stoppingToken);
                        continue;
                    }

                    // 模拟数据读取逻辑
                    var data = await device.ReadAsync<object>("address");
                    _logger.Info($"读取数据: {data}");
                }

                await Task.Delay(_options.Interval, stoppingToken); // 使用配置中的轮询间隔
            }

            _logger.Info("轮询服务停止");
        }
    }

    /// <summary>
    /// 轮询服务的配置选项。
    /// </summary>
    public class PollingServiceOptions
    {
        public int Interval { get; set; } = 1000; // 默认轮询间隔为 1000 毫秒
    }
}