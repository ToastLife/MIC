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
    public class LogQueryService : ILogQueryService
    {
        private readonly string _connectionString;

        public LogQueryService()
        {
            // 指向与 NLog 相同的数据库文件
            string dbPath = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "mic_data.db");
            _connectionString = $"Data Source={dbPath};";
        }

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