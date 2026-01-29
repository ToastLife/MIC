using MIC.Core.Interfaces;
using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;

namespace MIC.Services
{
    /// <summary>
    /// 高性能异步采集引擎
    /// </summary>
    public class PollingService
    {
        private readonly DeviceManager _deviceManager;
        private readonly ILoggerService _logger;
        private CancellationTokenSource _cts;
        private bool _isPolling = false;

        // 定义数据到达事件，UI订阅此事件即可更新
        public event Action<string, string, object> DataReceived;

        public PollingService(DeviceManager deviceManager, ILoggerService logger)
        {
            _deviceManager = deviceManager;
            _logger = logger;
        }

        public void Start()
        {
            if (_isPolling) return;
            _isPolling = true;
            _cts = new CancellationTokenSource();

            // 开启后台采集任务
            Task.Run(() => PollLoop(_cts.Token));
            _logger.Info("采集引擎已启动");
        }

        public void Stop()
        {
            _cts?.Cancel();
            _isPolling = false;
            _logger.Info("采集引擎已停止");
        }

        private async Task PollLoop(CancellationToken token)
        {
            while (!token.IsCancellationRequested)
            {
                foreach (var device in _deviceManager.GetAllDevices())
                {
                    if (!device.IsConnected)
                    {
                        // 自动重连逻辑
                        await Task.Run(() => device.Connect());
                        continue;
                    }

                    // 这里可以根据 dashboard.json 配置的地址进行读取
                    // 简化演示：假设我们要读取一些预设地址
                    try
                    {
                        // 示例：异步读取
                        var val = await device.ReadAsync<short>("40001");
                        DataReceived?.Invoke(device.DeviceId, "40001", val);
                    }
                    catch (Exception ex)
                    {
                        _logger.Error($"读取设备 {device.DeviceId} 失败: {ex.Message}");
                    }
                }

                // 采集频率控制，例如 500ms
                await Task.Delay(500, token);
            }
        }
    }
}