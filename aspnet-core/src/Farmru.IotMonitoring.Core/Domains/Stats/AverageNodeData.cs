using Abp.Domain.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Farmru.IotMonitoring.Domains.Stats
{
    public class AverageNodeData : Entity<Guid>
    {
        public virtual decimal? AvgSoilTemperature { get; set; }
        public virtual decimal? AvgSoilPH { get; set; }
        public virtual decimal? AvgMoisture { get; set; }
        public virtual decimal? AvgPhosphorus { get; set; }
        public virtual decimal? AvgPotassium { get; set; }
        public virtual decimal? AvgNitrogen { get; set; }
        public virtual decimal? AvgSolarPanelVoltage { get; set; }
        public virtual decimal? AvgBatteryVoltage { get; set; }
    }
}
