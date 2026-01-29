using MIC.Core.Interfaces;
using MIC.Models.DTOs;
using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;

namespace MIC.Services
{
    public class WorkflowEngine
    {
        private readonly DeviceManager _deviceManager;
        private readonly ILoggerService _logger; // 修复 CS0103：定义私有字段
        private CancellationTokenSource _cts;

        // UI 分离的关键：定义高性能事件
        public event EventHandler<WorkflowEventArgs> StepChanged;
        public event Action<bool, string> WorkflowCompleted;

        public WorkflowEngine(DeviceManager dm, ILoggerService logger)
        {
            _deviceManager = dm;
            _logger = logger; // 构造函数赋值
        }

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

        // 保留泛型写入写法
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

        // 补全 PollUntilMatch
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

        public void Stop() => _cts?.Cancel();
    }
}