using Abp.Domain.Entities.Auditing;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Farmru.IotMonitoring.Domain
{
    public class NodeData : FullAuditedEntity<Guid>
    {
        public virtual string? SoilTemperature { get; set; }
        public virtual string? SoilPH { get; set; }
        public virtual string? Moisture { get; set; }
        public virtual string? Phosphorus { get; set; }
        public virtual string? Potassium { get; set; }
        public virtual string? Nitrogen { get; set; }
        public virtual long? Latitude { get; set; }
        public virtual long? Longitude { get; set; }
        public virtual long? SolarPanelVoltage { get; set; }
        public virtual long? BatteryVoltage { get; set; }
    }
}
