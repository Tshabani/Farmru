using Abp.Application.Services;
using Abp.Application.Services.Dto;
using Farmru.IotMonitoring.Services.Incidents.Dto;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace Farmru.IotMonitoring.Services.Incidents
{
    public interface IIncidentAppService : IApplicationService
    {
        Task<PagedResultDto<IncidentDto>> GetIncidents(PagedIncidentResultRequestDto input);
        Task<IncidentDetailDto> GetIncident(EntityDto<Guid> input);
        Task<IncidentDashboardDto> GetDashboard();
        Task<List<IncidentDto>> GetActiveIncidents();
        Task<List<IncidentDto>> GetMyAssignedIncidents();
        Task<List<TechnicianDispatchSuggestionDto>> GetDispatchSuggestions(EntityDto<Guid> input);
        Task<IncidentDto> Create(CreateIncidentInput input);
        Task<IncidentDto> Assign(AssignIncidentInput input);
        Task<IncidentDto> Acknowledge(IncidentActionInput input);
        Task<IncidentDto> StartWork(IncidentActionInput input);
        Task<IncidentDto> MarkWaitingOnParts(IncidentActionInput input);
        Task<IncidentDto> Escalate(IncidentActionInput input);
        Task<IncidentDto> Resolve(IncidentActionInput input);
        Task<IncidentDto> Close(IncidentActionInput input);
        Task<IncidentDto> Cancel(IncidentActionInput input);
        Task<IncidentDto> Reopen(IncidentActionInput input);
        Task<IncidentDto> ChangeStatus(ChangeIncidentStatusInput input);
        Task AddComment(AddIncidentCommentInput input);
        Task<IncidentAttachmentDto> UploadAttachment(UploadIncidentAttachmentInput input);
    }
}
