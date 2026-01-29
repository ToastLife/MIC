using MIC.Core.Interfaces;
using NLog;
using System;

namespace MIC.Infrastructure.Logging
{
    /// <summary>
    /// NLog 日志实现。
    /// </summary>
    public class NLogLogger : ILoggerService
    {
        private readonly ILogger _logger;

        public NLogLogger()
        {
            _logger = LogManager.GetCurrentClassLogger();
        }

        public void Info(string message)
        {
            _logger.Info(message);
        }

        public void Error(string message, Exception ex = null)
        {
            _logger.Error(ex, message);
        }

        public void Warn(string message)
        {
            _logger.Warn(message);
        }

        public void Debug(string message)
        {
            _logger.Debug(message);
        }
    }
}