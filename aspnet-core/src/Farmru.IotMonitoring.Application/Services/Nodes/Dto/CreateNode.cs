using Abp.AutoMapper;
using Farmru.IotMonitoring.Domains.Nodes;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Farmru.IotMonitoring.Services.Nodes.Dto
{
    [AutoMap(typeof(Node))]
    public class CreateNode
    {
        public string SerialNumber { get; set; }
        public int TenantId { get; set; }
    }
}
