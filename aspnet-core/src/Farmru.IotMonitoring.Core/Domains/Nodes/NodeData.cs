using Abp.Domain.Entities;
using Farmru.IotMonitoring.Domains;
using Abp.Domain.Entities.Auditing;
using System;

namespace Farmru.IotMonitoring.Domains.Nodes
{
    /// <summary>
    /// Telemetry reading aggregate root (high-volume, separate from device lifecycle).
    /// </summary>
    public class NodeData : FullAuditedAggregateRoot<Guid>, IMustHaveTenant
    {
        protected NodeData()
        {
        }

        public virtual int TenantId { get; set; }
        public virtual Node Node { get; private set; }
        public virtual string SoilTemperature { get; private set; }
        public virtual string SoilPH { get; private set; }
        public virtual string Moisture { get; private set; }
        public virtual string Phosphorus { get; private set; }
        public virtual string Potassium { get; private set; }
        public virtual string Nitrogen { get; private set; }
        public virtual long? Latitude { get; private set; }
        public virtual long? Longitude { get; private set; }
        public virtual long? SolarPanelVoltage { get; private set; }
        public virtual long? BatteryVoltage { get; private set; }
        public virtual long? Conductivity { get; private set; }
        public virtual DateTime? LoggingTime { get; private set; }

        public static NodeData RecordFromDevice(
            Node node,
            string soilTemperature,
            string soilPH,
            string moisture,
            string phosphorus,
            string potassium,
            string nitrogen,
            long? latitude,
            long? longitude,
            long? solarPanelVoltage,
            long? batteryVoltage,
            long? conductivity,
            DateTime? loggingTime)
        {
            if (node == null)
            {
                throw new DomainRuleException("Device is required for telemetry.");
            }

            return new NodeData
            {
                TenantId = node.TenantId,
                Node = node,
                SoilTemperature = soilTemperature,
                SoilPH = soilPH,
                Moisture = moisture,
                Phosphorus = phosphorus,
                Potassium = potassium,
                Nitrogen = nitrogen,
                Latitude = latitude,
                Longitude = longitude,
                SolarPanelVoltage = solarPanelVoltage,
                BatteryVoltage = batteryVoltage,
                Conductivity = conductivity,
                LoggingTime = loggingTime ?? DateTime.UtcNow
            };
        }
    }
}
