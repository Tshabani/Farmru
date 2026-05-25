using Abp.Domain.Entities;
using Abp.Domain.Entities.Auditing;
using Farmru.IotMonitoring.Domains;
using Farmru.IotMonitoring.Domains.Facilities;
using Farmru.IotMonitoring.Domains.Nodes;
using Farmru.IotMonitoring.Domains.Persons;
using System;
using System.Collections.Generic;

namespace Farmru.IotMonitoring.Domains.Incidents
{
    public partial class Incident : FullAuditedAggregateRoot<Guid>, IMustHaveTenant
    {
        protected Incident()
        {
            Assignments = new List<IncidentAssignment>();
            Timeline = new List<IncidentTimelineEvent>();
            Attachments = new List<IncidentAttachment>();
        }

        public int TenantId { get; set; }
        public virtual string Title { get; private set; }
        public virtual string Description { get; private set; }
        public virtual IncidentStatus Status { get; private set; }
        public virtual IncidentPriority Priority { get; private set; }
        public virtual IncidentSlaStatus SlaStatus { get; private set; }
        public virtual int EscalationLevel { get; private set; }
        public virtual bool IsEscalated { get; private set; }
        public virtual DateTime CreatedDate { get; private set; }
        public virtual DateTime? AssignedAt { get; private set; }
        public virtual DateTime? AcknowledgedAt { get; private set; }
        public virtual DateTime? FirstResponseAt { get; private set; }
        public virtual DateTime? ResolvedDate { get; private set; }
        public virtual DateTime? ClosedDate { get; private set; }
        public virtual DateTime ResponseDueAt { get; private set; }
        public virtual DateTime ResolutionDueAt { get; private set; }
        public virtual bool SlaResponseBreached { get; private set; }
        public virtual bool SlaResolutionBreached { get; private set; }
        public virtual string ResolutionNotes { get; private set; }
        public virtual string AssignedTeamName { get; private set; }
        public virtual Person CreatedBy { get; private set; }
        public virtual Person AssignedTo { get; private set; }
        public virtual Facility Facility { get; private set; }
        public virtual Guid? FacilityId { get; private set; }
        public virtual Guid? NodeId { get; private set; }
        public virtual Guid? RelatedAlertId { get; private set; }

        public virtual ICollection<IncidentAssignment> Assignments { get; protected set; }
        public virtual ICollection<IncidentTimelineEvent> Timeline { get; protected set; }
        public virtual ICollection<IncidentAttachment> Attachments { get; protected set; }

        public static Incident Report(
            int tenantId,
            string title,
            string description,
            IncidentPriority priority,
            Person createdBy,
            Facility facility = null,
            Guid? nodeId = null,
            Guid? relatedAlertId = null)
        {
            var incident = new Incident();
            incident.TenantId = tenantId;
            incident.SetTitle(title);
            incident.Description = description?.Trim();
            incident.Priority = priority;
            incident.Status = IncidentStatus.Open;
            incident.SlaStatus = IncidentSlaStatus.OnTrack;
            incident.CreatedDate = DateTime.UtcNow;
            incident.CreatedBy = createdBy;
            incident.Facility = facility;
            incident.FacilityId = facility?.Id;
            incident.NodeId = nodeId;
            incident.RelatedAlertId = relatedAlertId;
            incident.ApplySlaDeadlines();
            return incident;
        }

        public virtual void UpdateDetails(string title, string description, IncidentPriority priority)
        {
            if (!string.IsNullOrWhiteSpace(title))
            {
                SetTitle(title);
            }

            Description = description?.Trim();
            if (Priority != priority && !IsTerminal())
            {
                Priority = priority;
                ApplySlaDeadlines();
            }
        }

        private void SetTitle(string title)
        {
            var trimmed = title?.Trim();
            if (string.IsNullOrWhiteSpace(trimmed))
            {
                throw new DomainRuleException("Incident title is required.");
            }

            Title = trimmed;
        }

        public virtual void ApplySlaDeadlines()
        {
            var (response, resolution) = IncidentSlaDefaults.ForPriority(Priority);
            ResponseDueAt = CreatedDate.AddMinutes(response);
            ResolutionDueAt = CreatedDate.AddMinutes(resolution);
        }

        public virtual bool IsTerminal() =>
            Status == IncidentStatus.Resolved ||
            Status == IncidentStatus.Closed ||
            Status == IncidentStatus.Cancelled;
    }
}
