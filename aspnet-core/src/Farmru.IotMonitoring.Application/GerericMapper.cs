using Farmru.IotMonitoring.Domain;
using Farmru.IotMonitoring.Helpers;
using Farmru.IotMonitoring.Services.NodeData.Dto;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Farmru.IotMonitoring
{
    /// <summary>
    /// 
    /// </summary>
    public class GerericMapper : ProfileHelper
    {
        /// <summary>
        /// 
        /// </summary>
        public GerericMapper()
        {
            CreateMap<CreateNodeData, NodeData>();
            CreateMap<NodeData, NodeDataDto>();
            CreateMap<NodeDataDto, NodeData>();
        }
    }
}
