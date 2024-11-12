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
        public virtual double? AvgSoilTemperature { get; set; }
        public virtual double? AvgSoilPH { get; set; }
        public virtual double? AvgMoisture { get; set; }
        public virtual double? AvgPhosphorus { get; set; }
        public virtual double? AvgPotassium { get; set; }
        public virtual double? AvgNitrogen { get; set; }
        public virtual double? AvgSolarPanelVoltage { get; set; }
        public virtual double? AvgBatteryVoltage { get; set; }
    }
}
