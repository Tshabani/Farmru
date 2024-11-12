using Abp.Application.Services.Dto;
using Abp.AutoMapper;
using Abp.Domain.Entities;
using Farmru.IotMonitoring.Helpers;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Farmru.IotMonitoring.Services.NodeData.Dto
{
    [AutoMap(typeof(Domains.Nodes.NodeData))]
    public class NodeDataDto : EntityDto<Guid>
    {
        public string? SoilTemperature { get; set; }
        public string? SoilPH { get; set; }
        public string? Moisture { get; set; }
        public string? Phosphorus { get; set; }
        public string? Potassium { get; set; }
        public string? Nitrogen { get; set; }
        public long? Latitude { get; set; }
        public long? Longitude { get; set; }
        public long? SolarPanelVoltage { get; set; }
        public long? BatteryVoltage { get; set; }
        public long? Conductivity { get; set; }
        public DateTime? LoggingTime { get; set; }
        public EntityWithDisplayNameDto<Guid?> Node { get; set; }
    }
}
