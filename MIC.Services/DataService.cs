using MIC.Core.Interfaces;
using MIC.Infrastructure.Database;
using MIC.Models.Entities;
using System;
using System.Threading.Tasks;

namespace MIC.Services
{
    /// <summary>
    /// 数据服务。负责将设备数据、告警和系统日志持久化到数据库（SQLite）
    /// </summary>
    public class DataService
    {
        private readonly SqliteDapperHelper _db;
        private readonly ILoggerService _logger;

        /// <summary>
        /// 初始化数据服务
        /// </summary>
        /// <param name="db">数据库辅助类（Dapper 封装）</param>
        /// <param name="logger">日志服务</param>
        public DataService(SqliteDapperHelper db, ILoggerService logger)
        {
            _db = db;
            _logger = logger;
        }

        /// <summary>
        /// 记录告警到数据库。包含设备 ID、消息、级别和时间戳
        /// </summary>
        /// <param name="deviceId">设备 ID</param>
        /// <param name="message">告警消息</param>
        /// <param name="level">告警级别（默认 "Error"）</param>
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
        /// 记录设备历史数据到数据库。保存设备读取的实时数据用于数据分析和回放
        /// </summary>
        /// <param name="deviceId">设备 ID</param>
        /// <param name="address">寄存器地址</param>
        /// <param name="value">读取的数值</param>
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
        /// 记录系统日志到数据库。用于事件溯源和审计
        /// </summary>
        /// <param name="level">日志级别（如 "Info"、"Warning"、"Error"）</param>
        /// <param name="message">日志消息</param>
        /// <param name="user">操作用户（默认 "System"）</param>
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