using Abp.Dependency;
using Farmru.IotMonitoring.Domains.Stats;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Farmru.IotMonitoring.Repository
{
    /// <summary>
    /// 
    /// </summary>
    public interface INodeDataRepository : ITransientDependency
    {
        /// <summary>
        /// 
        /// </summary>
        /// <returns></returns>
        Task<AverageNodeData> GetAverageNodeDataAsync();
    }
}
