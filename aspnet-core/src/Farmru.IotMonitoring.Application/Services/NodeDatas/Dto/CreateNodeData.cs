using Abp.AutoMapper;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Farmru.IotMonitoring.Services.NodeData.Dto
{
    [AutoMap(typeof(Domains.Nodes.NodeData))]
    public class CreateNodeData
    {
        public string? SoilTemperature { get; set; }
        public string? SoilPH { get; set; }
        public string? Moisture { get; set; }
        public string? Phosphorus { get; set; }
        public string? Potassium { get; set; }
        public string? Nitrogen { get; set; }
        public string? SerialNumber { get; set; }
        public long? Latitude { get; set; }
        public long? Longitude { get; set; }
        public long? SolarPanelVoltage { get; set; }
        public long? BatteryVoltage { get; set; }
        public DateTime? LoggingTime { get; set; }
    }
}
