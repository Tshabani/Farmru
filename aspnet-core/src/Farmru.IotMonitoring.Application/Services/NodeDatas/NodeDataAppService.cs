using Abp.Application.Services;
using Abp.Domain.Repositories;
using Farmru.IotMonitoring.Services.NodeData.Dto;
using Farmru.IotMonitoring.Users.Dto;
using System;
using System.Threading.Tasks;
using Farmru.IotMonitoring.Domains.Nodes;
using Abp.UI;
using System.Collections.Generic;
using System.Linq;
using Abp.Application.Services.Dto;
using Microsoft.EntityFrameworkCore;

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
        public async Task<PagedResultDto<NodeDataDto>> GetNodeDataByNodeId(Guid nodeId, PagedNodeDataResultRequestDto input)
        {
            // Ensure the node exists
            var nodeExists = await _nodeRepository.FirstOrDefaultAsync(x => x.Id == nodeId);
            if (nodeExists == null)
            {
                throw new UserFriendlyException("Node not found");
            }

            // Determine the date range based on predefined period or custom range
            DateTime? startDate = input.StartDate;
            DateTime? endDate = input.EndDate;

            if (!string.IsNullOrEmpty(input.PredefinedPeriod))
            {
                switch (input.PredefinedPeriod.ToLower())
                {
                    case "today":
                        startDate = DateTime.Today;
                        endDate = DateTime.Today.AddDays(1).AddTicks(-1);
                        break;
                    case "week":
                        startDate = DateTime.Today.AddDays(-7);
                        endDate = DateTime.Today.AddDays(1).AddTicks(-1);
                        break;
                    case "month":
                        startDate = DateTime.Today.AddMonths(-1);
                        endDate = DateTime.Today.AddDays(1).AddTicks(-1);
                        break;
                    case "year":
                        startDate = DateTime.Today.AddYears(-1);
                        endDate = DateTime.Today.AddDays(1).AddTicks(-1);
                        break;
                }
            }

            // Query filtered data
            var query = Repository.GetAll()
                .Where(r => r.Node != null && r.Node.Id == nodeId); // Filter by navigation property

            if (startDate.HasValue && endDate.HasValue)
            {
                query = query.Where(r => r.CreationTime >= startDate && r.CreationTime <= endDate);
            }

            // Get total count for pagination
            var totalCount = await query.CountAsync();

            // Apply pagination and fetch results
            var nodeData = await query
                .OrderByDescending(c => c.CreationTime)
                .Skip(input.SkipCount)
                .Take(input.MaxResultCount)
                .ToListAsync();

            // Map and return paginated results
            return new PagedResultDto<NodeDataDto>(
                totalCount,
                ObjectMapper.Map<List<NodeDataDto>>(nodeData)
            );
        }

    }
}
