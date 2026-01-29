using MIC.Core.Interfaces;
using Microsoft.Extensions.Hosting;
using System;
using System.Threading;
using System.Threading.Tasks;

namespace MIC.Services
{
    /// <summary>
    /// 轮询服务，改造为 IHostedService 实现。
    /// </summary>
    public class PollingService : IHostedService
    {
        private readonly DeviceManager _deviceManager;
        private readonly ILoggerService _logger;
        private CancellationTokenSource _cts;
        private Task _pollingTask;

        public PollingService(DeviceManager deviceManager, ILoggerService logger)
        {
            _deviceManager = deviceManager;
            _logger = logger;
        }

        public Task StartAsync(CancellationToken cancellationToken)
        {
            _logger.Info("轮询服务启动");
            _cts = CancellationTokenSource.CreateLinkedTokenSource(cancellationToken);
            _pollingTask = Task.Run(() => PollLoop(_cts.Token));
            return Task.CompletedTask;
        }

        public async Task StopAsync(CancellationToken cancellationToken)
        {
            _logger.Info("轮询服务停止");
            _cts.Cancel();
            if (_pollingTask != null)
            {
                await _pollingTask;
            }
        }

        private async Task PollLoop(CancellationToken token)
        {
            while (!token.IsCancellationRequested)
            {
                foreach (var device in _deviceManager.GetAllDevices())
                {
                    if (!device.IsConnected)
                    {
                        await Task.Run(() => device.Connect());
                        continue;
                    }
                    // 模拟数据读取逻辑
                    var data = await device.ReadAsync<object>("address");
                    _logger.Info($"读取数据: {data}");
                }
                await Task.Delay(1000, token); // 每秒轮询一次
            }
        }
    }
}