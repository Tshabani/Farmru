using Abp.Application.Services;
using Abp.Application.Services.Dto;
using Farmru.IotMonitoring.Services.Nodes.Dto;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace Farmru.IotMonitoring.Services.Nodes
{
    public interface INodeAppService : IApplicationService
    {
        Task<List<NodeDto>> GetMyNodes();
        Task<NodeDetailDto> GetDetail(EntityDto<Guid> input);
        Task<NodeDto> Activate(EntityDto<Guid> input);
        Task<NodeDto> Deactivate(EntityDto<Guid> input);
        Task<NodeDto> AssignToFacility(AssignNodeToFacilityInput input);
        Task<NodeDto> ReplaceDevice(ReplaceNodeInput input);
        Task<List<NodeReplacementHistoryDto>> GetReplacementHistory(EntityDto<Guid> input);
    }
}
