namespace MIC.Models.DTOs
{
    public class SystemSettings
    {
        public string AppName { get; set; } = "MIC 上位机框架";
        public string LastUser { get; set; }
        public bool AutoStartPolling { get; set; } = false;
        public string ThemeColor { get; set; } = "Default";
    }
}