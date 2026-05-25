using Abp.Application.Services.Dto;
using Abp.Authorization;
using Abp.Domain.Repositories;
using Abp.Linq.Extensions;
using Abp.Runtime.Session;
using Abp.UI;
using Farmru.IotMonitoring.Authorization;
using Farmru.IotMonitoring.Domains;
using Farmru.IotMonitoring.Domains.Facilities;
using Farmru.IotMonitoring.Domains.Incidents;
using Farmru.IotMonitoring.Domains.Persons;
using Farmru.IotMonitoring.Geo;
using Farmru.IotMonitoring.Helpers;
using Farmru.IotMonitoring.Incidents;
using Farmru.IotMonitoring.Services.Incidents.Dto;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Linq.Dynamic.Core;
using System.Threading.Tasks;

namespace Farmru.IotMonitoring.Services.Incidents
{
    [AbpAuthorize(PermissionNames.Pages_Incidents)]
    public class IncidentAppService : IotMonitoringAppServiceBase, IIncidentAppService
    {
        private static readonly IncidentStatus[] KanbanStatuses =
        {
            IncidentStatus.Open,
            IncidentStatus.Assigned,
            IncidentStatus.InProgress,
            IncidentStatus.Escalated,
            IncidentStatus.Resolved,
            IncidentStatus.Closed
        };

        private readonly IRepository<Incident, Guid> _incidentRepository;
        private readonly IRepository<Person, Guid> _personRepository;
        private readonly IRepository<Facility, Guid> _facilityRepository;
        private readonly IIncidentRealtimeNotifier _realtimeNotifier;

        public IncidentAppService(
            IRepository<Incident, Guid> incidentRepository,
            IRepository<Person, Guid> personRepository,
            IRepository<Facility, Guid> facilityRepository,
            IIncidentRealtimeNotifier realtimeNotifier)
        {
            _incidentRepository = incidentRepository;
            _personRepository = personRepository;
            _facilityRepository = facilityRepository;
            _realtimeNotifier = realtimeNotifier;
        }

        public async Task<PagedResultDto<IncidentDto>> GetIncidents(PagedIncidentResultRequestDto input)
        {
            var query = BuildFilteredQuery(input);
            var totalCount = await query.CountAsync();
            var items = await query
                .OrderBy(string.IsNullOrWhiteSpace(input.Sorting) ? "CreatedDate desc" : input.Sorting)
                .PageBy(input)
                .ToListAsync();

            return new PagedResultDto<IncidentDto>(totalCount, items.Select(MapToDto).ToList());
        }

        public async Task<IncidentDetailDto> GetIncident(EntityDto<Guid> input)
        {
            var incident = await GetIncidentQuery()
                .FirstOrDefaultAsync(i => i.Id == input.Id);

            if (incident == null)
            {
                throw new UserFriendlyException(L("IncidentNotFound"));
            }

            return MapToDetailDto(incident);
        }

        public async Task<IncidentDashboardDto> GetDashboard()
        {
            var now = DateTime.UtcNow;
            var active = await _incidentRepository.GetAll()
                .Include(i => i.AssignedTo)
                .Include(i => i.CreatedBy)
                .Include(i => i.Facility)
                .Where(i => i.Status != IncidentStatus.Closed && i.Status != IncidentStatus.Cancelled)
                .ToListAsync();

            var resolved = await _incidentRepository.GetAll()
                .Where(i => i.ResolvedDate.HasValue && i.ResolvedDate >= now.AddDays(-30))
                .ToListAsync();

            var responded = active.Where(i => i.FirstResponseAt.HasValue).ToList();
            var avgResponse = responded.Any()
                ? responded.Average(i => (i.FirstResponseAt.Value - i.CreatedDate).TotalMinutes)
                : 0;
            var avgResolution = resolved.Any()
                ? resolved.Average(i => (i.ResolvedDate.Value - i.CreatedDate).TotalMinutes)
                : 0;

            var slaMet = resolved.Count(i => !i.SlaResolutionBreached);
            var slaCompliance = resolved.Count > 0 ? slaMet * 100.0 / resolved.Count : 100;

            return new IncidentDashboardDto
            {
                TotalActive = active.Count,
                OverdueCount = active.Count(i => !i.IsTerminal() && now > i.ResolutionDueAt),
                SlaBreachedCount = active.Count(i => i.SlaStatus == IncidentSlaStatus.Breached),
                EscalatedCount = active.Count(i => i.IsEscalated || i.Status == IncidentStatus.Escalated),
                UnassignedCount = active.Count(i => i.AssignedTo == null && i.Status == IncidentStatus.Open),
                AverageResponseMinutes = Math.Round(avgResponse, 1),
                AverageResolutionMinutes = Math.Round(avgResolution, 1),
                SlaCompliancePercent = Math.Round(slaCompliance, 1),
                Kanban = KanbanStatuses.Select(status => new IncidentKanbanColumnDto
                {
                    Status = status,
                    Items = active.Where(i => i.Status == status).OrderByDescending(i => i.Priority).ThenBy(i => i.CreatedDate)
                        .Select(MapToDto).ToList()
                }).ToList(),
                TechnicianWorkload = active
                    .Where(i => i.AssignedTo != null)
                    .GroupBy(i => i.AssignedTo.Id)
                    .Select(g => new TechnicianWorkloadDto
                    {
                        PersonId = g.Key,
                        DisplayName = g.First().AssignedTo.FullName,
                        ActiveAssignments = g.Count()
                    })
                    .OrderByDescending(w => w.ActiveAssignments)
                    .Take(20)
                    .ToList()
            };
        }

        public async Task<List<IncidentDto>> GetActiveIncidents()
        {
            var items = await _incidentRepository.GetAll()
                .Include(i => i.AssignedTo)
                .Include(i => i.CreatedBy)
                .Include(i => i.Facility)
                .Where(i => i.Status != IncidentStatus.Closed && i.Status != IncidentStatus.Cancelled)
                .OrderByDescending(i => i.Priority)
                .ThenBy(i => i.CreatedDate)
                .Take(200)
                .ToListAsync();

            return items.Select(MapToDto).ToList();
        }

        public async Task<List<IncidentDto>> GetMyAssignedIncidents()
        {
            var person = await GetCurrentPersonAsync();
            if (person == null)
            {
                return new List<IncidentDto>();
            }

            var items = await _incidentRepository.GetAll()
                .Include(i => i.AssignedTo)
                .Include(i => i.CreatedBy)
                .Include(i => i.Facility)
                .Where(i => i.AssignedTo != null && i.AssignedTo.Id == person.Id)
                .Where(i => i.Status != IncidentStatus.Closed && i.Status != IncidentStatus.Cancelled)
                .OrderByDescending(i => i.Priority)
                .ThenBy(i => i.CreatedDate)
                .ToListAsync();

            return items.Select(MapToDto).ToList();
        }

        public async Task<List<TechnicianDispatchSuggestionDto>> GetDispatchSuggestions(EntityDto<Guid> input)
        {
            var incident = await GetIncidentQuery().FirstOrDefaultAsync(i => i.Id == input.Id);
            if (incident == null)
            {
                throw new UserFriendlyException(L("IncidentNotFound"));
            }
            var technicians = await _personRepository.GetAll()
                .Include(p => p.User)
                .Where(p => p.User != null)
                .ToListAsync();

            var assignedActive = await _incidentRepository.GetAll()
                .Include(i => i.AssignedTo)
                .Where(i => i.AssignedTo != null)
                .Where(i => i.Status != IncidentStatus.Resolved && i.Status != IncidentStatus.Closed && i.Status != IncidentStatus.Cancelled)
                .ToListAsync();
            var activeCounts = assignedActive
                .GroupBy(i => i.AssignedTo.Id)
                .Select(g => new { PersonId = g.Key, Count = g.Count() })
                .ToList();

            var suggestions = technicians.Select(p =>
            {
                double? distanceKm = null;
                if (incident.Latitude.HasValue && incident.Longitude.HasValue)
                {
                    var facility = incident.Facility;
                    if (facility?.Latitude != null && facility.Longitude != null)
                    {
                        distanceKm = GeoCoordinateHelper.DistanceMeters(
                            incident.Latitude.Value, incident.Longitude.Value,
                            facility.Latitude.Value, facility.Longitude.Value) / 1000.0;
                    }
                }

                return new TechnicianDispatchSuggestionDto
                {
                    PersonId = p.Id,
                    DisplayName = p.FullName,
                    ActiveIncidents = activeCounts.FirstOrDefault(c => c.PersonId == p.Id)?.Count ?? 0,
                    DistanceKm = distanceKm.HasValue ? Math.Round(distanceKm.Value, 2) : null
                };
            })
            .OrderBy(s => s.ActiveIncidents)
            .ThenBy(s => s.DistanceKm ?? double.MaxValue)
            .Take(15)
            .ToList();

            return suggestions;
        }

        [AbpAuthorize(PermissionNames.Pages_Incidents_Manage)]
        public async Task<IncidentDto> Create(CreateIncidentInput input)
        {
            var tenantId = AbpSession.GetTenantId();
            Person createdBy = null;
            var person = await GetCurrentPersonAsync();
            if (person != null)
            {
                createdBy = person;
            }

            Facility facility = null;
            if (input.FacilityId.HasValue)
            {
                facility = await _facilityRepository.FirstOrDefaultAsync(input.FacilityId.Value);
            }

            var incident = Incident.Report(
                tenantId,
                input.Title,
                input.Description,
                input.Priority,
                createdBy,
                facility,
                input.NodeId,
                input.RelatedAlertId);

            if (input.Latitude.HasValue && input.Longitude.HasValue)
            {
                incident.SetLocation(input.Latitude, input.Longitude);
            }

            incident.RecordTimeline(IncidentTimelineEventType.Created, "Incident reported", input.Description);
            await _incidentRepository.InsertAsync(incident);
            await CurrentUnitOfWork.SaveChangesAsync();

            var loaded = await GetIncidentQuery().FirstAsync(i => i.Id == incident.Id);
            return await NotifyAndReturnAsync(loaded, "created");
        }

        [AbpAuthorize(PermissionNames.Pages_Incidents_Manage)]
        public async Task<IncidentDto> Assign(AssignIncidentInput input)
        {
            var incident = await GetIncidentOrThrowAsync(input.IncidentId);
            var assignee = await _personRepository.FirstOrDefaultAsync(input.PersonId);
            if (assignee == null)
            {
                throw new UserFriendlyException(L("PersonNotFound"));
            }

            try
            {
                incident.Assign(assignee, AbpSession.GetUserId(), input.TeamName, input.DispatchNotes);
                incident.RecordTimeline(IncidentTimelineEventType.Assigned, "Technician assigned", assignee.FullName);
                await _incidentRepository.UpdateAsync(incident);
                await CurrentUnitOfWork.SaveChangesAsync();
                var loaded = await GetIncidentQuery().FirstAsync(i => i.Id == incident.Id);
                return await NotifyAndReturnAsync(loaded, "assigned");
            }
            catch (DomainRuleException ex)
            {
                throw new UserFriendlyException(ex.Message);
            }
        }

        public async Task<IncidentDto> Acknowledge(IncidentActionInput input)
        {
            var incident = await GetIncidentOrThrowAsync(input.IncidentId);
            try
            {
                incident.Acknowledge(AbpSession.GetUserId());
                await SaveAndNotifyAsync(incident, "acknowledged");
                var loaded = await GetIncidentQuery().FirstAsync(i => i.Id == incident.Id);
                return MapToDto(loaded);
            }
            catch (DomainRuleException ex)
            {
                throw new UserFriendlyException(ex.Message);
            }
        }

        public async Task<IncidentDto> StartWork(IncidentActionInput input)
        {
            return await ExecuteActionAsync(input.IncidentId, i => i.StartWork(), "started");
        }

        public async Task<IncidentDto> MarkWaitingOnParts(IncidentActionInput input)
        {
            return await ExecuteActionAsync(input.IncidentId, i => i.MarkWaitingOnParts(), "waitingOnParts");
        }

        [AbpAuthorize(PermissionNames.Pages_Incidents_Manage)]
        public async Task<IncidentDto> Escalate(IncidentActionInput input)
        {
            return await ExecuteActionAsync(input.IncidentId, i => i.Escalate(input.Notes), "escalated");
        }

        public async Task<IncidentDto> Resolve(IncidentActionInput input)
        {
            return await ExecuteActionAsync(input.IncidentId, i => i.Resolve(input.Notes, AbpSession.GetUserId()), "resolved");
        }

        [AbpAuthorize(PermissionNames.Pages_Incidents_Manage)]
        public async Task<IncidentDto> Close(IncidentActionInput input)
        {
            return await ExecuteActionAsync(input.IncidentId, i => i.Close(), "closed");
        }

        [AbpAuthorize(PermissionNames.Pages_Incidents_Manage)]
        public async Task<IncidentDto> Cancel(IncidentActionInput input)
        {
            return await ExecuteActionAsync(input.IncidentId, i => i.Cancel(input.Notes), "cancelled");
        }

        [AbpAuthorize(PermissionNames.Pages_Incidents_Manage)]
        public async Task<IncidentDto> Reopen(IncidentActionInput input)
        {
            return await ExecuteActionAsync(input.IncidentId, i => i.Reopen(), "reopened");
        }

        [AbpAuthorize(PermissionNames.Pages_Incidents_Manage)]
        public async Task<IncidentDto> ChangeStatus(ChangeIncidentStatusInput input)
        {
            var incident = await GetIncidentOrThrowAsync(input.IncidentId);
            try
            {
                switch (input.Status)
                {
                    case IncidentStatus.InProgress:
                        incident.StartWork();
                        break;
                    case IncidentStatus.WaitingOnParts:
                        incident.MarkWaitingOnParts();
                        break;
                    case IncidentStatus.Escalated:
                        incident.Escalate(input.Notes);
                        break;
                    case IncidentStatus.Resolved:
                        incident.Resolve(input.Notes, AbpSession.GetUserId());
                        break;
                    case IncidentStatus.Closed:
                        incident.Close();
                        break;
                    case IncidentStatus.Cancelled:
                        incident.Cancel(input.Notes);
                        break;
                    case IncidentStatus.Open:
                        if (incident.Status == IncidentStatus.Resolved || incident.Status == IncidentStatus.Closed)
                        {
                            incident.Reopen();
                        }
                        else
                        {
                            throw new UserFriendlyException(L("InvalidIncidentStatusTransition"));
                        }
                        break;
                    default:
                        throw new UserFriendlyException(L("InvalidIncidentStatusTransition"));
                }

                await SaveAndNotifyAsync(incident, "statusChanged");
                var loaded = await GetIncidentQuery().FirstAsync(i => i.Id == incident.Id);
                return MapToDto(loaded);
            }
            catch (DomainRuleException ex)
            {
                throw new UserFriendlyException(ex.Message);
            }
        }

        public async Task AddComment(AddIncidentCommentInput input)
        {
            var incident = await GetIncidentOrThrowAsync(input.IncidentId);
            try
            {
                incident.AddComment(input.Comment, AbpSession.GetUserId());
                await SaveAndNotifyAsync(incident, "commentAdded");
            }
            catch (DomainRuleException ex)
            {
                throw new UserFriendlyException(ex.Message);
            }
        }

        public async Task<IncidentAttachmentDto> UploadAttachment(UploadIncidentAttachmentInput input)
        {
            if (input.FileBytes == null || input.FileBytes.Length == 0)
            {
                throw new UserFriendlyException(L("AttachmentFileRequired"));
            }

            var incident = await GetIncidentOrThrowAsync(input.IncidentId);
            var tenantId = AbpSession.GetTenantId();
            var safeName = Path.GetFileName(input.FileName ?? "upload.bin");
            var relativePath = Path.Combine(tenantId.ToString(), incident.Id.ToString(), $"{Guid.NewGuid()}_{safeName}");
            var root = Path.Combine(Directory.GetCurrentDirectory(), "wwwroot", "incident-attachments");
            var fullPath = Path.Combine(root, relativePath);
            Directory.CreateDirectory(Path.GetDirectoryName(fullPath));
            await File.WriteAllBytesAsync(fullPath, input.FileBytes);

            try
            {
                var attachment = incident.AddAttachment(
                    safeName,
                    input.ContentType ?? "application/octet-stream",
                    relativePath.Replace('\\', '/'),
                    input.FileBytes.Length,
                    input.Caption);

                await _incidentRepository.UpdateAsync(incident);
                await CurrentUnitOfWork.SaveChangesAsync();
                await _realtimeNotifier.NotifyIncidentChangedAsync(MapToDto(incident), "attachmentAdded");

                return new IncidentAttachmentDto
                {
                    Id = attachment.Id,
                    FileName = attachment.FileName,
                    ContentType = attachment.ContentType,
                    FileSizeBytes = attachment.FileSizeBytes,
                    Caption = attachment.Caption,
                    CreationTime = attachment.CreationTime
                };
            }
            catch (DomainRuleException ex)
            {
                throw new UserFriendlyException(ex.Message);
            }
        }

        private async Task<IncidentDto> ExecuteActionAsync(Guid incidentId, Action<Incident> action, string actionName)
        {
            var incident = await GetIncidentOrThrowAsync(incidentId);
            try
            {
                action(incident);
                return await SaveAndNotifyAsync(incident, actionName);
            }
            catch (DomainRuleException ex)
            {
                throw new UserFriendlyException(ex.Message);
            }
        }

        private async Task<IncidentDto> SaveAndNotifyAsync(Incident incident, string action)
        {
            await _incidentRepository.UpdateAsync(incident);
            await CurrentUnitOfWork.SaveChangesAsync();
            var loaded = await GetIncidentQuery().FirstAsync(i => i.Id == incident.Id);
            return await NotifyAndReturnAsync(loaded, action);
        }

        private async Task<IncidentDto> NotifyAndReturnAsync(Incident incident, string action)
        {
            var dto = MapToDto(incident);
            await _realtimeNotifier.NotifyIncidentChangedAsync(dto, action);
            return dto;
        }

        private IQueryable<Incident> GetIncidentQuery() =>
            _incidentRepository.GetAll()
                .Include(i => i.AssignedTo)
                .Include(i => i.CreatedBy)
                .Include(i => i.Facility)
                .Include(i => i.Timeline)
                .Include(i => i.Assignments).ThenInclude(a => a.AssignedPerson)
                .Include(i => i.Attachments);

        private IQueryable<Incident> BuildFilteredQuery(PagedIncidentResultRequestDto input)
        {
            var query = GetIncidentQuery();
            var now = DateTime.UtcNow;

            if (input.Status.HasValue)
            {
                query = query.Where(i => i.Status == input.Status.Value);
            }

            if (input.Priority.HasValue)
            {
                query = query.Where(i => i.Priority == input.Priority.Value);
            }

            if (input.AssignedPersonId.HasValue)
            {
                query = query.Where(i => i.AssignedTo != null && i.AssignedTo.Id == input.AssignedPersonId.Value);
            }

            if (input.FacilityId.HasValue)
            {
                query = query.Where(i => i.FacilityId == input.FacilityId.Value);
            }

            if (input.OverdueOnly == true)
            {
                query = query.Where(i => !i.IsTerminal() && now > i.ResolutionDueAt);
            }

            return query;
        }

        private async Task<Incident> GetIncidentOrThrowAsync(Guid id)
        {
            var incident = await GetIncidentQuery().FirstOrDefaultAsync(i => i.Id == id);
            if (incident == null)
            {
                throw new UserFriendlyException(L("IncidentNotFound"));
            }

            return incident;
        }

        private async Task<Person> GetCurrentPersonAsync()
        {
            if (!AbpSession.UserId.HasValue)
            {
                return null;
            }

            return await _personRepository.GetAll()
                .Include(p => p.User)
                .FirstOrDefaultAsync(p => p.User != null && p.User.Id == AbpSession.UserId.Value);
        }

        private IncidentDto MapToDto(Incident incident) =>
            new IncidentDto
            {
                Id = incident.Id,
                TenantId = incident.TenantId,
                Title = incident.Title,
                Description = incident.Description,
                Status = incident.Status,
                Priority = incident.Priority,
                SlaStatus = incident.SlaStatus,
                EscalationLevel = incident.EscalationLevel,
                IsEscalated = incident.IsEscalated,
                CreatedDate = incident.CreatedDate,
                AssignedAt = incident.AssignedAt,
                AcknowledgedAt = incident.AcknowledgedAt,
                FirstResponseAt = incident.FirstResponseAt,
                ResolvedDate = incident.ResolvedDate,
                ResponseDueAt = incident.ResponseDueAt,
                ResolutionDueAt = incident.ResolutionDueAt,
                SlaResponseBreached = incident.SlaResponseBreached,
                SlaResolutionBreached = incident.SlaResolutionBreached,
                ResolutionNotes = incident.ResolutionNotes,
                AssignedTeamName = incident.AssignedTeamName,
                Latitude = incident.Latitude,
                Longitude = incident.Longitude,
                FacilityId = incident.FacilityId,
                NodeId = incident.NodeId,
                CreatedBy = incident.CreatedBy != null
                    ? new EntityWithDisplayNameDto<Guid?> { Id = incident.CreatedBy.Id, DisplayText = incident.CreatedBy.FullName }
                    : null,
                AssignedTo = incident.AssignedTo != null
                    ? new EntityWithDisplayNameDto<Guid?> { Id = incident.AssignedTo.Id, DisplayText = incident.AssignedTo.FullName }
                    : null,
                Facility = incident.Facility != null
                    ? new EntityWithDisplayNameDto<Guid?> { Id = incident.Facility.Id, DisplayText = incident.Facility.Name }
                    : null
            };

        private IncidentDetailDto MapToDetailDto(Incident incident)
        {
            var dto = new IncidentDetailDto();
            var baseDto = MapToDto(incident);
            dto.Id = baseDto.Id;
            dto.TenantId = baseDto.TenantId;
            dto.Title = baseDto.Title;
            dto.Description = baseDto.Description;
            dto.Status = baseDto.Status;
            dto.Priority = baseDto.Priority;
            dto.SlaStatus = baseDto.SlaStatus;
            dto.EscalationLevel = baseDto.EscalationLevel;
            dto.IsEscalated = baseDto.IsEscalated;
            dto.CreatedDate = baseDto.CreatedDate;
            dto.AssignedAt = baseDto.AssignedAt;
            dto.AcknowledgedAt = baseDto.AcknowledgedAt;
            dto.FirstResponseAt = baseDto.FirstResponseAt;
            dto.ResolvedDate = baseDto.ResolvedDate;
            dto.ResponseDueAt = baseDto.ResponseDueAt;
            dto.ResolutionDueAt = baseDto.ResolutionDueAt;
            dto.SlaResponseBreached = baseDto.SlaResponseBreached;
            dto.SlaResolutionBreached = baseDto.SlaResolutionBreached;
            dto.ResolutionNotes = baseDto.ResolutionNotes;
            dto.AssignedTeamName = baseDto.AssignedTeamName;
            dto.Latitude = baseDto.Latitude;
            dto.Longitude = baseDto.Longitude;
            dto.FacilityId = baseDto.FacilityId;
            dto.NodeId = baseDto.NodeId;
            dto.CreatedBy = baseDto.CreatedBy;
            dto.AssignedTo = baseDto.AssignedTo;
            dto.Facility = baseDto.Facility;
            dto.Timeline = incident.Timeline
                .OrderByDescending(t => t.CreationTime)
                .Select(t => new IncidentTimelineEventDto
                {
                    Id = t.Id,
                    EventType = t.EventType,
                    Title = t.Title,
                    Description = t.Description,
                    CreationTime = t.CreationTime,
                    CreatorUserId = t.CreatorUserId
                }).ToList();
            dto.Assignments = incident.Assignments
                .OrderByDescending(a => a.AssignedAt)
                .Select(a => new IncidentAssignmentDto
                {
                    Id = a.Id,
                    AssignedPerson = a.AssignedPerson != null
                        ? new EntityWithDisplayNameDto<Guid?> { Id = a.AssignedPerson.Id, DisplayText = a.AssignedPerson.FullName }
                        : null,
                    AssignedTeamName = a.AssignedTeamName,
                    AssignedAt = a.AssignedAt,
                    UnassignedAt = a.UnassignedAt,
                    DispatchNotes = a.DispatchNotes,
                    IsActive = a.IsActive
                }).ToList();
            dto.Attachments = incident.Attachments
                .OrderByDescending(a => a.CreationTime)
                .Select(a => new IncidentAttachmentDto
                {
                    Id = a.Id,
                    FileName = a.FileName,
                    ContentType = a.ContentType,
                    FileSizeBytes = a.FileSizeBytes,
                    Caption = a.Caption,
                    CreationTime = a.CreationTime
                }).ToList();
            return dto;
        }
    }
}
