using Abp.Application.Services.Dto;
using Farmru.IotMonitoring.Domains.Incidents;
using Farmru.IotMonitoring.Helpers;
using System;
using System.Collections.Generic;

namespace Farmru.IotMonitoring.Services.Incidents.Dto
{
    public class IncidentDto : EntityDto<Guid>
    {
        public int TenantId { get; set; }
        public string Title { get; set; }
        public string Description { get; set; }
        public IncidentStatus Status { get; set; }
        public IncidentPriority Priority { get; set; }
        public IncidentSlaStatus SlaStatus { get; set; }
        public int EscalationLevel { get; set; }
        public bool IsEscalated { get; set; }
        public DateTime CreatedDate { get; set; }
        public DateTime? AssignedAt { get; set; }
        public DateTime? AcknowledgedAt { get; set; }
        public DateTime? FirstResponseAt { get; set; }
        public DateTime? ResolvedDate { get; set; }
        public DateTime ResponseDueAt { get; set; }
        public DateTime ResolutionDueAt { get; set; }
        public bool SlaResponseBreached { get; set; }
        public bool SlaResolutionBreached { get; set; }
        public string ResolutionNotes { get; set; }
        public string AssignedTeamName { get; set; }
        public decimal? Latitude { get; set; }
        public decimal? Longitude { get; set; }
        public Guid? FacilityId { get; set; }
        public Guid? NodeId { get; set; }
        public EntityWithDisplayNameDto<Guid?> CreatedBy { get; set; }
        public EntityWithDisplayNameDto<Guid?> AssignedTo { get; set; }
        public EntityWithDisplayNameDto<Guid?> Facility { get; set; }
    }

    public class IncidentDetailDto : IncidentDto
    {
        public List<IncidentTimelineEventDto> Timeline { get; set; } = new();
        public List<IncidentAssignmentDto> Assignments { get; set; } = new();
        public List<IncidentAttachmentDto> Attachments { get; set; } = new();
    }

    public class IncidentTimelineEventDto
    {
        public Guid Id { get; set; }
        public IncidentTimelineEventType EventType { get; set; }
        public string Title { get; set; }
        public string Description { get; set; }
        public DateTime CreationTime { get; set; }
        public long? CreatorUserId { get; set; }
    }

    public class IncidentAssignmentDto
    {
        public Guid Id { get; set; }
        public EntityWithDisplayNameDto<Guid?> AssignedPerson { get; set; }
        public string AssignedTeamName { get; set; }
        public DateTime AssignedAt { get; set; }
        public DateTime? UnassignedAt { get; set; }
        public string DispatchNotes { get; set; }
        public bool IsActive { get; set; }
    }

    public class IncidentAttachmentDto
    {
        public Guid Id { get; set; }
        public string FileName { get; set; }
        public string ContentType { get; set; }
        public long FileSizeBytes { get; set; }
        public string Caption { get; set; }
        public DateTime CreationTime { get; set; }
    }

    public class CreateIncidentInput
    {
        public string Title { get; set; }
        public string Description { get; set; }
        public IncidentPriority Priority { get; set; }
        public Guid? FacilityId { get; set; }
        public Guid? NodeId { get; set; }
        public Guid? RelatedAlertId { get; set; }
        public decimal? Latitude { get; set; }
        public decimal? Longitude { get; set; }
    }

    public class AssignIncidentInput
    {
        public Guid IncidentId { get; set; }
        public Guid PersonId { get; set; }
        public string TeamName { get; set; }
        public string DispatchNotes { get; set; }
    }

    public class IncidentActionInput
    {
        public Guid IncidentId { get; set; }
        public string Notes { get; set; }
    }

    public class ChangeIncidentStatusInput
    {
        public Guid IncidentId { get; set; }
        public IncidentStatus Status { get; set; }
        public string Notes { get; set; }
    }

    public class AddIncidentCommentInput
    {
        public Guid IncidentId { get; set; }
        public string Comment { get; set; }
    }

    public class IncidentDashboardDto
    {
        public int TotalActive { get; set; }
        public int OverdueCount { get; set; }
        public int SlaBreachedCount { get; set; }
        public int EscalatedCount { get; set; }
        public int UnassignedCount { get; set; }
        public double AverageResponseMinutes { get; set; }
        public double AverageResolutionMinutes { get; set; }
        public double SlaCompliancePercent { get; set; }
        public List<IncidentKanbanColumnDto> Kanban { get; set; } = new();
        public List<TechnicianWorkloadDto> TechnicianWorkload { get; set; } = new();
    }

    public class IncidentKanbanColumnDto
    {
        public IncidentStatus Status { get; set; }
        public List<IncidentDto> Items { get; set; } = new();
    }

    public class TechnicianWorkloadDto
    {
        public Guid PersonId { get; set; }
        public string DisplayName { get; set; }
        public int ActiveAssignments { get; set; }
    }

    public class TechnicianDispatchSuggestionDto
    {
        public Guid PersonId { get; set; }
        public string DisplayName { get; set; }
        public int ActiveIncidents { get; set; }
        public double? DistanceKm { get; set; }
    }

    public class PagedIncidentResultRequestDto : PagedAndSortedResultRequestDto
    {
        public IncidentStatus? Status { get; set; }
        public IncidentPriority? Priority { get; set; }
        public bool? OverdueOnly { get; set; }
        public Guid? AssignedPersonId { get; set; }
        public Guid? FacilityId { get; set; }
    }

    public class UploadIncidentAttachmentInput
    {
        public Guid IncidentId { get; set; }
        public string FileName { get; set; }
        public string ContentType { get; set; }
        public byte[] FileBytes { get; set; }
        public string Caption { get; set; }
    }
}
