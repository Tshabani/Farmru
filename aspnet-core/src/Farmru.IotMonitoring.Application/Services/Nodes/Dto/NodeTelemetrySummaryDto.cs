using System;

namespace Farmru.IotMonitoring.Services.Nodes.Dto
{
    public class NodeTelemetrySummaryDto
    {
        public DateTime? LastReadingTime { get; set; }
        public string Moisture { get; set; }
        public string SoilTemperature { get; set; }
        public string SoilPH { get; set; }
        public string Nitrogen { get; set; }
        public string Phosphorus { get; set; }
        public string Potassium { get; set; }
        public long? Latitude { get; set; }
        public long? Longitude { get; set; }
        public int TotalReadings { get; set; }
    }
}
