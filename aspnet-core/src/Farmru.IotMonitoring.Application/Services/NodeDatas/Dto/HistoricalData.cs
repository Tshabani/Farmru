using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Farmru.IotMonitoring.Services.NodeDatas.Dto
{
    public class HistoricalData
    {
        public string Day { get; set; }
        public double SoilTemp { get; set; }
        public double Moisture { get; set; }
        public double PH { get; set; }
    }
}
