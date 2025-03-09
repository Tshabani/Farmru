using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Farmru.IotMonitoring.Services.NodeDatas.Dto
{
    public class CurrentReadings
    {
        public double SoilTemp { get; set; }
        public double SoilPH { get; set; }
        public double Moisture { get; set; }
        public double Phosphorus { get; set; }
        public double Potassium { get; set; }
        public double Nitrogen { get; set; }
        public double SolarVoltage { get; set; }
        public double BatteryVoltage { get; set; }
    }
}
