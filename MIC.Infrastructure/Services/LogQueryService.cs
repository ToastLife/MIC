using Dapper;
using MIC.Core.Entities;
using MIC.Core.Interfaces;
using Microsoft.Data.Sqlite;
using System;
using System.Collections.Generic;
using System.Data.SQLite;
using System.IO;
using System.Text;
using System.Threading.Tasks;

namespace MIC.Infrastructure.Services
{
    /// <summary>
    /// 日志查询服务。提供从 SQLite 数据库中查询系统日志的功能。
    /// </summary>
    public class LogQueryService : ILogQueryService
    {
        /// <summary>
        /// SQLite 数据库连接字符串
        /// </summary>
        private readonly string _connectionString;

        /// <summary>
        /// 初始化日志查询服务。设置数据库文件路径和连接字符串。
        /// </summary>
        public LogQueryService()
        {
            // 指向与 NLog 相同的数据库文件
            string dbPath = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "mic_data.db");
            _connectionString = $"Data Source={dbPath};";
        }

        /// <summary>
        /// 异步查询系统日志。
        /// </summary>
        /// <param name="start">查询的起始日期</param>
        /// <param name="end">查询的结束日期</param>
        /// <param name="level">日志级别（如 "Info"、"Error"）</param>
        /// <param name="keyword">查询关键字（匹配消息或记录器）</param>
        /// <returns>符合条件的系统日志集合</returns>
        public async Task<IEnumerable<SystemLog>> GetLogsAsync(DateTime start, DateTime end, string level, string keyword)
        {
            using (var conn = new SqliteConnection(_connectionString))
            {
                var sql = new StringBuilder("SELECT * FROM SystemLogs WHERE Date BETWEEN @Start AND @End");

                // 动态构建查询
                if (!string.IsNullOrEmpty(level) && level != "ALL")
                {
                    sql.Append(" AND Level = @Level");
                }

                if (!string.IsNullOrEmpty(keyword))
                {
                    sql.Append(" AND (Message LIKE @Keyword OR Logger LIKE @Keyword)");
                }

                // 加上排序和限制，防止一次加载过多
                sql.Append(" ORDER BY Date DESC LIMIT 1000");

                return await conn.QueryAsync<SystemLog>(sql.ToString(), new
                {
                    Start = start,
                    End = end,
                    Level = level,
                    Keyword = $"%{keyword}%"
                });
            }
        }
    }
}