using Farmru.IotMonitoring.Domains.Nodes;
using Farmru.IotMonitoring.Helpers;
using Farmru.IotMonitoring.Services.NodeData.Dto;
using Farmru.IotMonitoring.Services.Nodes.Dto;
using System;

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
            CreateMap<NodeData, NodeDataDto>()
                .ForMember(u => u.Node, opt => opt.MapFrom(r => r.Node != null ? new EntityWithDisplayNameDto<Guid?> { Id = r.Node.Id, DisplayText = r.Node.SerialNumber } : null));
            CreateMap<NodeDataDto, NodeData>();
            
            CreateMap<CreateNode, Node>();
            CreateMap<Node, NodeDto>();
            CreateMap<NodeDto, Node>();
        }
    }
}
