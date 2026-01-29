using MIC.Core.Interfaces;
using MIC.Models.DTOs;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Options;
using System.Threading;
using System.Threading.Tasks;

namespace MIC.Services
{
    /// <summary>
    /// 工作流引擎，继承 BackgroundService。
    /// </summary>
    public class WorkflowEngine : BackgroundService
    {
        private readonly DeviceManager _deviceManager;
        private readonly ILoggerService _logger;
        private readonly WorkflowEngineOptions _options;

        public WorkflowEngine(DeviceManager deviceManager, ILoggerService logger, IOptions<WorkflowEngineOptions> options)
        {
            _deviceManager = deviceManager;
            _logger = logger;
            _options = options.Value;
        }

        protected override async Task ExecuteAsync(CancellationToken stoppingToken)
        {
            _logger.Info("工作流引擎启动");

            while (!stoppingToken.IsCancellationRequested)
            {
                // 模拟工作流逻辑
                _logger.Info("执行工作流步骤");
                await Task.Delay(_options.StepInterval, stoppingToken); // 使用配置中的步骤间隔
            }

            _logger.Info("工作流引擎停止");
        }
    }

    /// <summary>
    /// 工作流引擎的配置选项。
    /// </summary>
    public class WorkflowEngineOptions
    {
        public int StepInterval { get; set; } = 1000; // 默认步骤间隔为 1000 毫秒
    }
}