using Abp.Domain.Entities;
using Abp.Domain.Entities.Auditing;
using Microsoft.AspNetCore.Components.Web.Virtualization;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Farmru.IotMonitoring.Domains.Nodes
{
    public class NodeData : FullAuditedEntity<Guid>, IMustHaveTenant
    {
        public virtual string SoilTemperature { get; set; }
        public virtual string SoilPH { get; set; }
        public virtual string Moisture { get; set; }
        public virtual string Phosphorus { get; set; }
        public virtual string Potassium { get; set; }
        public virtual string Nitrogen { get; set; }
        public virtual long? Latitude { get; set; }
        public virtual long? Longitude { get; set; }
        public virtual long? SolarPanelVoltage { get; set; }
        public virtual long? BatteryVoltage { get; set; }
        /// <summary>
        /// The node that this data belongs to
        /// </summary>
        public virtual Node? Node { get; set; }
        public virtual int TenantId { get; set; }
        public virtual DateTime? LoggingTime { get; set; }
    }
}
