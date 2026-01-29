using MIC.Core.Interfaces;
using MIC.Models.DTOs;
using Microsoft.Extensions.Hosting;
using System;
using System.Threading;
using System.Threading.Tasks;

namespace MIC.Services
{
    /// <summary>
    /// 工作流引擎，改造为 IHostedService 实现。
    /// </summary>
    public class WorkflowEngine : IHostedService
    {
        private readonly DeviceManager _deviceManager;
        private readonly ILoggerService _logger;
        private CancellationTokenSource _cts;
        private Task _workflowTask;

        /// <summary>
        /// 当工作流执行到某个步骤时触发的事件
        /// </summary>
        public event EventHandler<WorkflowEventArgs> StepChanged;
        
        /// <summary>
        /// 工作流完成时触发的事件。参数 (bool: 是否成功，string: 完成信息)
        /// </summary>
        public event Action<bool, string> WorkflowCompleted;

        /// <summary>
        /// 初始化工作流引擎
        /// </summary>
        /// <param name="dm">设备管理器，用于访问设备驱动</param>
        /// <param name="logger">日志服务</param>
        public WorkflowEngine(DeviceManager dm, ILoggerService logger)
        {
            _deviceManager = dm;
            _logger = logger;
        }

        public Task StartAsync(CancellationToken cancellationToken)
        {
            _logger.Info("工作流引擎启动");
            _cts = CancellationTokenSource.CreateLinkedTokenSource(cancellationToken);
            _workflowTask = Task.Run(() => RunWorkflow(_cts.Token));
            return Task.CompletedTask;
        }

        public async Task StopAsync(CancellationToken cancellationToken)
        {
            _logger.Info("工作流引擎停止");
            _cts.Cancel();
            if (_workflowTask != null)
            {
                await _workflowTask;
            }
        }

        private async Task RunWorkflow(CancellationToken token)
        {
            while (!token.IsCancellationRequested)
            {
                // 模拟工作流逻辑
                _logger.Info("执行工作流步骤");
                await Task.Delay(1000, token); // 每秒执行一次
            }
        }

        /// <summary>
        /// 异步执行工作流。按照定义的步骤顺序执行，支持条件分支和错误处理
        /// </summary>
        /// <param name="workflow">要执行的工作流定义</param>
        public async Task RunAsync(WorkflowDefine workflow)
        {
            if (workflow == null || workflow.Steps.Count == 0) return;

            _cts = new CancellationTokenSource();
            int currentStepId = workflow.Steps[0].Id;

            try
            {
                _logger.Info($"开始执行流程: {workflow.Name}");

                while (currentStepId > 0 && !_cts.Token.IsCancellationRequested)
                {
                    var step = workflow.Steps.Find(s => s.Id == currentStepId);
                    if (step == null) break;

                    // 1. 触发 UI 更新事件 (不阻塞)
                    StepChanged?.Invoke(this, new WorkflowEventArgs
                    {
                        StepId = step.Id,
                        Message = step.Name,
                        Status = "Running"
                    });

                    // 2. 执行逻辑并获取结果
                    bool success = await ExecuteStepLogic(step, _cts.Token);

                    // 3. 决定下一步走向
                    if (success)
                    {
                        currentStepId = step.NextStepId;
                    }
                    else
                    {
                        _logger.Warn($"步骤 {step.Name} 失败");
                        if (step.ErrorStepId > 0)
                        {
                            currentStepId = step.ErrorStepId;
                        }
                        else
                        {
                            throw new Exception($"步骤 {step.Name} 失败且无配置跳转。");
                        }
                    }

                    if (currentStepId <= 0) break; // 正常结束标识
                }
                WorkflowCompleted?.Invoke(true, "流程执行完毕");
            }
            catch (Exception ex)
            {
                _logger.Error("流程执行异常", ex);
                WorkflowCompleted?.Invoke(false, ex.Message);
            }
        }

        /// <summary>
        /// 执行单个步骤的具体逻辑。根据步骤的 Action 类型调用相应的操作（写入、等待、延迟等）
        /// </summary>
        /// <param name="step">要执行的步骤</param>
        /// <param name="token">取消令牌，用于支持取消操作</param>
        /// <returns>步骤执行成功时返回 true</returns>
        private async Task<bool> ExecuteStepLogic(WorkflowStep step, CancellationToken token)
        {
            var driver = _deviceManager.GetDevice(step.DeviceId);
            if (driver == null) return false;

            switch (step.Action)
            {
                case ActionType.Write:
                    return await InvokeGenericWrite(driver, step.Address, step.TargetValue);

                case ActionType.WaitValue:
                    return await PollUntilMatch(driver, step, token);

                case ActionType.Delay:
                    await Task.Delay(int.Parse(step.TargetValue), token);
                    return true;

                default:
                    return false;
            }
        }

        /// <summary>
        /// 通用的泛型写入方法。根据值的类型自动转换为 bool、float 或 short
        /// </summary>
        /// <param name="driver">目标设备驱动</param>
        /// <param name="address">设备寄存器地址</param>
        /// <param name="value">要写入的值（字符串形式）</param>
        /// <returns>写入成功返回 true</returns>
        private async Task<bool> InvokeGenericWrite(IDeviceDriver driver, string address, string value)
        {
            try
            {
                if (value.ToLower() == "true" || value.ToLower() == "false")
                    return await driver.WriteAsync<bool>(address, bool.Parse(value));

                if (value.Contains("."))
                    return await driver.WriteAsync<float>(address, float.Parse(value));

                return await driver.WriteAsync<short>(address, short.Parse(value));
            }
            catch (Exception ex)
            {
                _logger.Error($"写入转换失败: {address}", ex);
                return false;
            }
        }

        /// <summary>
        /// 轮询设备直到读取的值与目标值匹配（带超时控制）。用于等待设备状态的步骤
        /// </summary>
        /// <param name="driver">目标设备驱动</param>
        /// <param name="step">工作流步骤（包含地址、目标值、超时时间）</param>
        /// <param name="token">取消令牌</param>
        /// <returns>值匹配返回 true，超时返回 false</returns>
        private async Task<bool> PollUntilMatch(IDeviceDriver driver, WorkflowStep step, CancellationToken token)
        {
            var start = DateTime.Now;
            while ((DateTime.Now - start).TotalMilliseconds < step.TimeoutMs)
            {
                if (token.IsCancellationRequested) return false;

                try
                {
                    var current = await driver.ReadAsync<string>(step.Address);
                    if (current != null && current.Trim().ToLower() == step.TargetValue.Trim().ToLower())
                        return true;
                }
                catch { /* 忽略读取抖动 */ }

                await Task.Delay(200, token); // 降低轮询频率保护性能
            }
            return false;
        }

        /// <summary>
        /// 停止当前正在执行的工作流。发送取消令牌给所有异步任务
        /// </summary>
        public void Stop() => _cts?.Cancel();
    }
}