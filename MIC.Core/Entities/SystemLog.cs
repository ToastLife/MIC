using System;

namespace MIC.Core.Entities
{
    /// <summary>
    /// 系统日志实体类。用于表示系统中的日志记录。
    /// </summary>
    public class SystemLog
    {
        /// <summary>
        /// 日志记录的唯一标识符
        /// </summary>
        public long Id { get; set; }

        /// <summary>
        /// 日志记录的时间戳
        /// </summary>
        public DateTime Date { get; set; }

        /// <summary>
        /// 日志级别（如 Info、Error）
        /// </summary>
        public string Level { get; set; }

        /// <summary>
        /// 记录器名称
        /// </summary>
        public string Logger { get; set; }

        /// <summary>
        /// 日志消息内容
        /// </summary>
        public string Message { get; set; }

        /// <summary>
        /// 异常信息（如果有）
        /// </summary>
        public string Exception { get; set; }
    }
}