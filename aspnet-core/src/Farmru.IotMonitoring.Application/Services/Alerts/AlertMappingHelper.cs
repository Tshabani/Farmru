using Farmru.IotMonitoring.Domains.Alerts;
using Farmru.IotMonitoring.Helpers;
using Farmru.IotMonitoring.Services.Alerts.Dto;
using System;

namespace Farmru.IotMonitoring.Services.Alerts
{
    internal static class AlertMappingHelper
    {
        public static AlertDto ToDto(Alert alert)
        {
            return new AlertDto
            {
                Id = alert.Id,
                TenantId = alert.TenantId,
                NodeId = alert.NodeId,
                FacilityId = alert.FacilityId,
                Node = alert.Node != null
                    ? new EntityWithDisplayNameDto<Guid?> { Id = alert.Node.Id, DisplayText = alert.Node.SerialNumber }
                    : null,
                Facility = alert.Facility != null
                    ? new EntityWithDisplayNameDto<Guid?> { Id = alert.Facility.Id, DisplayText = alert.Facility.Name }
                    : null,
                AlertType = alert.AlertType,
                Severity = alert.Severity,
                Title = alert.Title,
                Description = alert.Description,
                IsAcknowledged = alert.IsAcknowledged,
                AcknowledgedByUserId = alert.AcknowledgedByUserId,
                AcknowledgedAt = alert.AcknowledgedAt,
                IsResolved = alert.IsResolved,
                ResolvedByUserId = alert.ResolvedByUserId,
                ResolvedAt = alert.ResolvedAt,
                ResolutionNotes = alert.ResolutionNotes,
                TriggeredAt = alert.TriggeredAt,
                LastTriggeredAt = alert.LastTriggeredAt,
                IsActive = alert.IsActive,
                MetadataJson = alert.MetadataJson,
                SourceTelemetryId = alert.SourceTelemetryId
            };
        }
    }
}
