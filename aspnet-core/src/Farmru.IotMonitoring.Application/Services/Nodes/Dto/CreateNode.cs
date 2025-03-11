using Abp.AutoMapper;
using Farmru.IotMonitoring.Domains.Nodes;
using Farmru.IotMonitoring.Helpers;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Farmru.IotMonitoring.Services.Nodes.Dto
{
    /// <summary>
    /// 
    /// </summary>
    [AutoMap(typeof(Node))]
    public class CreateNode
    {
        /// <summary>
        /// 
        /// </summary>
        public string SerialNumber { get; set; }
        /// <summary>
        /// 
        /// </summary>
        public EntityWithDisplayNameDto<Guid?> Facility { get; set; }
        /// <summary>
        /// 
        /// </summary>
        public int TenantId { get; set; }
    }
}
