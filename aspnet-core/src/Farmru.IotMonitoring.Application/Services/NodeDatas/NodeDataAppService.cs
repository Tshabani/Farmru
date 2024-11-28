using Abp.Application.Services;
using Abp.Domain.Repositories;
using Farmru.IotMonitoring.Services.NodeData.Dto;
using Farmru.IotMonitoring.Users.Dto;
using System;
using System.Threading.Tasks;
using Farmru.IotMonitoring.Domains.Nodes;
using Abp.UI;
using System.Collections.Generic;

namespace Farmru.IotMonitoring.Services.NodeDatas
{
    public class NodeDataAppService : AsyncCrudAppService<Domains.Nodes.NodeData, NodeDataDto, Guid, PagedUserResultRequestDto, CreateNodeData, NodeDataDto>, INodeDataAppService
    {
        private readonly IRepository<Node, Guid> _nodeRepository;

        /// <summary>
        /// 
        /// </summary>
        /// <param name="repository"></param>
        public NodeDataAppService(IRepository<Domains.Nodes.NodeData, Guid> repository, IRepository<Node, Guid> nodeRepository) : base(repository)
        {
            _nodeRepository = nodeRepository;
        }

        public override async Task<NodeDataDto> CreateAsync(CreateNodeData input)
        {
            ArgumentNullException.ThrowIfNull(input);
            if (string.IsNullOrWhiteSpace(input.SerialNumber)) throw new ArgumentNullException("Serial Number is required");

            var node = await _nodeRepository.FirstOrDefaultAsync(x => x.SerialNumber == input.SerialNumber) ?? throw new UserFriendlyException("Node not found");
            
            var nodeData = new Domains.Nodes.NodeData();
            ObjectMapper.Map(input, nodeData);

            nodeData.Node = node;
            nodeData.TenantId = node.TenantId;

            nodeData = await Repository.InsertAsync(nodeData);
            return ObjectMapper.Map<NodeDataDto>(nodeData);
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="nodeId"></param>
        /// <returns></returns>
        public async Task<List<NodeDataDto>> GetNodeDataByNodeId(Guid nodeId)
        {
            var node = await _nodeRepository.FirstOrDefaultAsync(x => x.Id == nodeId) ?? throw new UserFriendlyException("Node not found");

            var nodeData = await Repository.GetAllListAsync(r => r.Node == node);
             
            return ObjectMapper.Map<List<NodeDataDto>>(nodeData);
        }
    }
}
