using Abp.Application.Services.Dto;
using Farmru.IotMonitoring.Domains.Alerts;
using Farmru.IotMonitoring.Helpers;
using System;

namespace Farmru.IotMonitoring.Services.Alerts.Dto
{
    public class AlertDto : EntityDto<Guid>
    {
        public int TenantId { get; set; }
        public Guid? NodeId { get; set; }
        public Guid? FacilityId { get; set; }
        public EntityWithDisplayNameDto<Guid?> Node { get; set; }
        public EntityWithDisplayNameDto<Guid?> Facility { get; set; }
        public AlertType AlertType { get; set; }
        public AlertSeverity Severity { get; set; }
        public string Title { get; set; }
        public string Description { get; set; }
        public bool IsAcknowledged { get; set; }
        public long? AcknowledgedByUserId { get; set; }
        public DateTime? AcknowledgedAt { get; set; }
        public bool IsResolved { get; set; }
        public long? ResolvedByUserId { get; set; }
        public DateTime? ResolvedAt { get; set; }
        public string ResolutionNotes { get; set; }
        public DateTime TriggeredAt { get; set; }
        public DateTime? LastTriggeredAt { get; set; }
        public bool IsActive { get; set; }
        public string MetadataJson { get; set; }
        public Guid? SourceTelemetryId { get; set; }
    }
}
