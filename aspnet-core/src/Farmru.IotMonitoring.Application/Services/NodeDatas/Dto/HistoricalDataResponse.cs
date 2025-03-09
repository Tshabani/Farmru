using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Farmru.IotMonitoring.Services.NodeDatas.Dto
{
    /// <summary>
    /// 
    /// </summary>
    public class HistoricalDataResponse
    {
        /// <summary>
        /// 
        /// </summary>
        public CurrentReadings CurrentReadings { get; set; }
        /// <summary>
        /// 
        /// </summary>
        public List<HistoricalData> historicalData { get; set; }
    }
}
