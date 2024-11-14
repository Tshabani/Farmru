using Abp.Application.Services;
using Abp.Application.Services.Dto;
using Abp.Domain.Repositories; 
using Farmru.IotMonitoring.Domains.Nodes;
using Farmru.IotMonitoring.Services.NodeData.Dto;
using Farmru.IotMonitoring.Services.Nodes.Dto;
using Farmru.IotMonitoring.Users.Dto;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Farmru.IotMonitoring.Services.Nodes
{
    public class NodeAppService : AsyncCrudAppService<Node, NodeDto, Guid, PagedResultRequestDto, CreateNode, NodeDto>, INodeAppService
    {
        public NodeAppService(IRepository<Node, Guid> repository) : base(repository)
        {
        }

        
    }
}
