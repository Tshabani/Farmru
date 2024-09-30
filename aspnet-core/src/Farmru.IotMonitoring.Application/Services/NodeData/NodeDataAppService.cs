using Abp.Application.Services;
using Abp.Domain.Repositories;
using Farmru.IotMonitoring.Services.NodeData.Dto;
using Farmru.IotMonitoring.Users.Dto;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Farmru.IotMonitoring.Services.NodeData
{
    public class NodeDataAppService : AsyncCrudAppService<Domain.NodeData, NodeDataDto, Guid,  PagedUserResultRequestDto, CreateNodeData, NodeDataDto>,INodeDataAppService
    {
        /// <summary>
        /// 
        /// </summary>
        /// <param name="repository"></param>
        public NodeDataAppService(IRepository<Domain.NodeData, Guid> repository) : base(repository)
        {
                
        }
    }
}
