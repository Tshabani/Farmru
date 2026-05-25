using Abp.Domain.Entities;
using System;

namespace Farmru.IotMonitoring.Domains.Stats
{
    /// <summary>
    /// Read-model statistics entity (not an aggregate root).
    /// </summary>
    public class AverageNodeData : Entity<Guid>
    {
        protected AverageNodeData()
        {
        }

        public virtual decimal? AvgSoilTemperature { get; private set; }
        public virtual decimal? AvgSoilPH { get; private set; }
        public virtual decimal? AvgMoisture { get; private set; }
        public virtual decimal? AvgPhosphorus { get; private set; }
        public virtual decimal? AvgPotassium { get; private set; }
        public virtual decimal? AvgNitrogen { get; private set; }
        public virtual decimal? AvgSolarPanelVoltage { get; private set; }
        public virtual decimal? AvgBatteryVoltage { get; private set; }
    }
}
