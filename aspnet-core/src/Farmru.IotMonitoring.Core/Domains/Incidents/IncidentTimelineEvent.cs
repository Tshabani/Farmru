using Abp.Domain.Entities;
using Abp.Domain.Entities.Auditing;
using System;

namespace Farmru.IotMonitoring.Domains.Incidents
{
    public class IncidentTimelineEvent : Entity<Guid>, IMustHaveTenant, ICreationAudited
    {
        protected IncidentTimelineEvent()
        {
        }

        public int TenantId { get; set; }
        public virtual Guid IncidentId { get; private set; }
        public virtual Incident Incident { get; private set; }
        public virtual IncidentTimelineEventType EventType { get; private set; }
        public virtual string Title { get; private set; }
        public virtual string Description { get; private set; }
        public virtual string MetadataJson { get; private set; }
        public virtual long? CreatorUserId { get; set; }
        public virtual DateTime CreationTime { get; set; }

        public static IncidentTimelineEvent Record(
            int tenantId,
            Incident incident,
            IncidentTimelineEventType eventType,
            string title,
            string description = null,
            string metadataJson = null)
        {
            return new IncidentTimelineEvent
            {
                Id = Guid.NewGuid(),
                TenantId = tenantId,
                Incident = incident,
                IncidentId = incident.Id,
                EventType = eventType,
                Title = title?.Trim() ?? eventType.ToString(),
                Description = description?.Trim(),
                MetadataJson = metadataJson,
                CreationTime = DateTime.UtcNow
            };
        }
    }
}
