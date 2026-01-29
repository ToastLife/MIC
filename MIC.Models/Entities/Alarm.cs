using System;

namespace MIC.Models.Entities
{
    public class Alarm
    {
        public int Id { get; set; }
        public string DeviceId { get; set; }
        public string Message { get; set; }
        public string Level { get; set; } // Info, Warning, Error
        public DateTime OccurredTime { get; set; }
    }
}