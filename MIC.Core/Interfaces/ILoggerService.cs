namespace MIC.Core.Interfaces
{
    /// <summary>
    /// 日志服务接口。定义应用程序日志的基本操作。应用程序通过此接口而非具体实现解耦日志库
    /// </summary>
    public interface ILoggerService
    {
        /// <summary>
        /// 记录信息级别的日志
        /// </summary>
        /// <param name="message">日志消息</param>
        void Info(string message);
        
        /// <summary>
        /// 记录警告级别的日志
        /// </summary>
        /// <param name="message">日志消息</param>
        void Warn(string message);
        
        /// <summary>
        /// 记录错误级别的日志。可选地包含异常信息
        /// </summary>
        /// <param name="message">日志消息</param>
        /// <param name="ex">相关的异常对象（可选）</param>
        void Error(string message, System.Exception ex = null);
        
        /// <summary>
        /// 记录调试级别的日志。用于开发和诊断
        /// </summary>
        /// <param name="message">日志消息</param>
        void Debug(string message);
    }
}