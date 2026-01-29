namespace MIC.Core.Interfaces
{
    public interface ILoggerService
    {
        void Info(string message);
        void Warn(string message);
        void Error(string message, System.Exception ex = null);
        void Debug(string message);
    }
}