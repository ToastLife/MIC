using MIC.Core.Interfaces;
using NLog;
using System;

namespace MIC.Infrastructure.Logging
{
    public class NLogLogger : ILoggerService
    {
        // 使用 NLog 的 LogManager
        private static readonly Logger _logger = LogManager.GetCurrentClassLogger();

        public void Debug(string message) => _logger.Debug(message);

        public void Error(string message, Exception ex = null)
        {
            if (ex != null) _logger.Error(ex, message);
            else _logger.Error(message);
        }

        public void Info(string message) => _logger.Info(message);

        public void Warn(string message)
        {
            _logger.Warn(message);
        }
    }
}