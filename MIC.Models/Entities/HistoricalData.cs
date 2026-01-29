using System;

namespace MIC.Models.Entities
{
    public class HistoricalData
    {
        public int Id { get; set; }
        public string DeviceId { get; set; }
        public string Address { get; set; }
        public string Value { get; set; }
        public DateTime RecordTime { get; set; }
    }
}