using Dapper;
using MIC.Core.Interfaces;
using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SQLite;
using System.IO;
using System.Threading.Tasks;

namespace MIC.Infrastructure.Database
{
    public class SqliteDapperHelper
    {
        private readonly string _connectionString;
        private readonly ILoggerService _logger;

        public SqliteDapperHelper(string dbPath, ILoggerService logger)
        {
            _logger = logger;
            // 构建连接字符串
            string fullDbPath = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, dbPath);
            _connectionString = $"Data Source={fullDbPath};Version=3;";

            InitializeDb(fullDbPath);
        }

        private void InitializeDb(string path)
        {
            if (!File.Exists(path))
            {
                SQLiteConnection.CreateFile(path);
                _logger.Info("Database file created.");

                // 初始化表结构
                using (var conn = GetConnection())
                {
                    conn.Execute(@"
                        CREATE TABLE IF NOT EXISTS Alarms (
                            Id INTEGER PRIMARY KEY AUTOINCREMENT,
                            DeviceId TEXT NOT NULL,
                            Message TEXT NOT NULL,
                            Level TEXT NOT NULL,
                            OccurredTime DATETIME DEFAULT CURRENT_TIMESTAMP
                        );
                        CREATE TABLE IF NOT EXISTS HistoricalData (
                            Id INTEGER PRIMARY KEY AUTOINCREMENT,
                            DeviceId TEXT,
                            Address TEXT,
                            Value TEXT,
                            RecordTime DATETIME DEFAULT CURRENT_TIMESTAMP
                        );
                    ");
                }
            }
        }

        private IDbConnection GetConnection()
        {
            return new SQLiteConnection(_connectionString);
        }

        // 插入数据的通用方法
        public async Task<int> ExecuteAsync(string sql, object param = null)
        {
            try
            {
                using (var conn = GetConnection())
                {
                    return await conn.ExecuteAsync(sql, param);
                }
            }
            catch (Exception ex)
            {
                _logger.Error("Database Execute Error", ex);
                return -1;
            }
        }

        // 查询数据的通用方法
        public async Task<IEnumerable<T>> QueryAsync<T>(string sql, object param = null)
        {
            using (var conn = GetConnection())
            {
                return await conn.QueryAsync<T>(sql, param);
            }
        }
    }
}