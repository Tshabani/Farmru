using Abp.Domain.Entities;
using Abp.Domain.Entities.Auditing;
using Farmru.IotMonitoring.Domains;
using Farmru.IotMonitoring.Domains.Facilities;
using Farmru.IotMonitoring.Domains.Nodes;
using System;

namespace Farmru.IotMonitoring.Domains.Alerts
{
    public partial class Alert : FullAuditedAggregateRoot<Guid>, IMustHaveTenant
    {
        protected Alert()
        {
        }

        public int TenantId { get; set; }
        public virtual Guid? NodeId { get; private set; }
        public virtual Node Node { get; private set; }
        public virtual Guid? FacilityId { get; private set; }
        public virtual Facility Facility { get; private set; }
        public virtual AlertType AlertType { get; private set; }
        public virtual AlertSeverity Severity { get; private set; }
        public virtual string Title { get; private set; }
        public virtual string Description { get; private set; }
        public virtual bool IsAcknowledged { get; private set; }
        public virtual long? AcknowledgedByUserId { get; private set; }
        public virtual DateTime? AcknowledgedAt { get; private set; }
        public virtual bool IsResolved { get; private set; }
        public virtual long? ResolvedByUserId { get; private set; }
        public virtual DateTime? ResolvedAt { get; private set; }
        public virtual string ResolutionNotes { get; private set; }
        public virtual DateTime TriggeredAt { get; private set; }
        public virtual DateTime? LastTriggeredAt { get; private set; }
        public virtual bool IsActive { get; private set; }
        public virtual string MetadataJson { get; private set; }
        public virtual Guid? SourceTelemetryId { get; private set; }
        public virtual int EscalationLevel { get; private set; }
        public virtual DateTime? EscalatedAt { get; private set; }
        public virtual DateTime? LastEscalatedAt { get; private set; }

        public static Alert Raise(
            int tenantId,
            Guid? nodeId,
            Guid? facilityId,
            AlertType alertType,
            AlertSeverity severity,
            string title,
            string description,
            Guid? sourceTelemetryId = null,
            string metadataJson = null)
        {
            if (string.IsNullOrWhiteSpace(title))
            {
                throw new DomainRuleException("Alert title is required.");
            }

            return new Alert
            {
                TenantId = tenantId,
                NodeId = nodeId,
                FacilityId = facilityId,
                AlertType = alertType,
                Severity = severity,
                Title = title.Trim(),
                Description = description?.Trim(),
                TriggeredAt = DateTime.UtcNow,
                LastTriggeredAt = DateTime.UtcNow,
                IsActive = true,
                IsResolved = false,
                IsAcknowledged = false,
                SourceTelemetryId = sourceTelemetryId,
                MetadataJson = metadataJson
            };
        }

        public virtual void AttachNode(Node node)
        {
            Node = node;
            NodeId = node?.Id;
            FacilityId = node?.Facility?.Id;
            Facility = node?.Facility;
        }

        public virtual void UpdateLastTriggered(AlertSeverity? newSeverity = null, string description = null, Guid? sourceTelemetryId = null)
        {
            if (IsResolved)
            {
                throw new DomainRuleException("Cannot re-trigger a resolved alert. Reopen it first.");
            }

            LastTriggeredAt = DateTime.UtcNow;
            IsActive = true;

            if (!string.IsNullOrWhiteSpace(description))
            {
                Description = description.Trim();
            }

            if (sourceTelemetryId.HasValue)
            {
                SourceTelemetryId = sourceTelemetryId;
            }

            if (newSeverity.HasValue && newSeverity.Value > Severity)
            {
                Escalate(newSeverity.Value);
            }
        }

        public virtual void Acknowledge(long userId)
        {
            if (IsResolved)
            {
                throw new DomainRuleException("Cannot acknowledge a resolved alert.");
            }

            IsAcknowledged = true;
            AcknowledgedByUserId = userId;
            AcknowledgedAt = DateTime.UtcNow;
        }

        public virtual void Resolve(long userId, string resolutionNotes = null)
        {
            if (IsResolved)
            {
                throw new DomainRuleException("Alert is already resolved.");
            }

            IsResolved = true;
            IsActive = false;
            ResolvedByUserId = userId;
            ResolvedAt = DateTime.UtcNow;
            ResolutionNotes = string.IsNullOrWhiteSpace(resolutionNotes) ? null : resolutionNotes.Trim();
        }

        public virtual void Reopen()
        {
            if (!IsResolved)
            {
                throw new DomainRuleException("Only resolved alerts can be reopened.");
            }

            IsResolved = false;
            IsActive = true;
            ResolvedByUserId = null;
            ResolvedAt = null;
            ResolutionNotes = null;
            LastTriggeredAt = DateTime.UtcNow;
        }

        public virtual void Escalate(AlertSeverity newSeverity)
        {
            RecordEscalation(newSeverity);
        }

        public virtual void RecordEscalation(AlertSeverity newSeverity)
        {
            if (IsResolved)
            {
                throw new DomainRuleException("Cannot escalate a resolved alert.");
            }

            if (newSeverity < Severity)
            {
                throw new DomainRuleException("Escalation severity must be equal or higher.");
            }

            Severity = newSeverity;
            IsActive = true;
            EscalationLevel++;
            var now = DateTime.UtcNow;
            EscalatedAt ??= now;
            LastEscalatedAt = now;
        }

        public virtual void AutoResolve(string reason = null)
        {
            if (IsResolved)
            {
                return;
            }

            IsResolved = true;
            IsActive = false;
            ResolvedAt = DateTime.UtcNow;
            ResolutionNotes = reason ?? "Auto-resolved: condition normalized.";
        }
    }
}
