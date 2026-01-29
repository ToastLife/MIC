using MIC.Core.Interfaces;
using NLog;
using System;

namespace MIC.Infrastructure.Logging
{
    /// <summary>
    /// NLog 日志服务实现。通过 NLog 框架提供日志功能，实现 ILoggerService 接口
    /// </summary>
    public class NLogLogger : ILoggerService
    {
        /// <summary>
        /// 使用 NLog 的 LogManager 获取当前类的 Logger 实例
        /// </summary>
        private static readonly Logger _logger = LogManager.GetCurrentClassLogger();

        /// <summary>
        /// 记录调试级别的日志
        /// </summary>
        /// <param name="message">日志消息</param>
        public void Debug(string message) => _logger.Debug(message);

        /// <summary>
        /// 记录错误级别的日志。如果提供异常对象会一并记录
        /// </summary>
        /// <param name="message">日志消息</param>
        /// <param name="ex">异常对象（可选）</param>
        public void Error(string message, Exception ex = null)
        {
            if (ex != null) _logger.Error(ex, message);
            else _logger.Error(message);
        }

        /// <summary>
        /// 记录信息级别的日志
        /// </summary>
        /// <param name="message">日志消息</param>
        public void Info(string message) => _logger.Info(message);

        /// <summary>
        /// 记录警告级别的日志
        /// </summary>
        /// <param name="message">日志消息</param>
        public void Warn(string message)
        {
            _logger.Warn(message);
        }
    }
}