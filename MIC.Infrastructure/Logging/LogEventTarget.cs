using NLog;
using NLog.Targets;
using System;

namespace MIC.Infrastructure.Logging
{
    // 自定义 NLog Target，用于 UI 实时显示
    [Target("LogEvent")]
    public class LogEventTarget : TargetWithLayout
    {
        // 定义日志到达事件
        public static event Action<LogEventInfo> OnLogReceived;

        protected override void Write(LogEventInfo logEvent)
        {
            // 每次 NLog 记录日志时，触发此事件
            OnLogReceived?.Invoke(logEvent);
        }
    }
}