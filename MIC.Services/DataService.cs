using MIC.Core.Interfaces;
using MIC.Infrastructure.Database;
using MIC.Models.Entities;
using System;
using System.Threading.Tasks;

namespace MIC.Services
{
    public class DataService
    {
        private readonly SqliteDapperHelper _db;
        private readonly ILoggerService _logger;

        public DataService(SqliteDapperHelper db, ILoggerService logger)
        {
            _db = db;
            _logger = logger;
        }

        /// <summary>
        /// 记录报警
        /// </summary>
        public async Task LogAlarmAsync(string deviceId, string message, string level = "Error")
        {
            try
            {
                string sql = "INSERT INTO Alarms (DeviceId, Message, Level, OccurredTime) VALUES (@DeviceId, @Message, @Level, @OccurredTime)";
                await _db.ExecuteAsync(sql, new
                {
                    DeviceId = deviceId,
                    Message = message,
                    Level = level,
                    OccurredTime = DateTime.Now
                });
            }
            catch (Exception ex)
            {
                _logger.Error("Failed to save alarm", ex);
            }
        }

        /// <summary>
        /// 记录历史数据
        /// </summary>
        public async Task LogHistoryAsync(string deviceId, string address, object value)
        {
            try
            {
                string sql = "INSERT INTO HistoricalData (DeviceId, Address, Value, RecordTime) VALUES (@DeviceId, @Address, @Value, @RecordTime)";
                await _db.ExecuteAsync(sql, new
                {
                    DeviceId = deviceId,
                    Address = address,
                    Value = value?.ToString() ?? "",
                    RecordTime = DateTime.Now
                });
            }
            catch (Exception ex)
            {
                _logger.Error("Failed to save history", ex);
            }
        }

        /// <summary>
        /// 记录系统日志
        /// </summary>
        public async Task SaveSystemLogAsync(string level, string message, string user = "System")
        {
            try
            {
                string sql = "INSERT INTO SystemLogs (LogLevel, Message, UserName, CreateTime) VALUES (@Level, @Msg, @User, @Time)";
                await _db.ExecuteAsync(sql, new
                {
                    Level = level,
                    Msg = message,
                    User = user,
                    Time = DateTime.Now
                });
            }
            catch (Exception ex)
            {
                // 此处不能再调用 Logger.Error，否则可能导致死循环，直接控制台输出
                Console.WriteLine("DB Log Error: " + ex.Message);
            }
        }
    }
}