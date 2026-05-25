using Abp.Domain.Entities;
using Abp.Domain.Entities.Auditing;
using Farmru.IotMonitoring.Domains.Persons;
using System;

namespace Farmru.IotMonitoring.Domains.Incidents
{
    public class IncidentAssignment : FullAuditedEntity<Guid>, IMustHaveTenant
    {
        protected IncidentAssignment()
        {
        }

        public int TenantId { get; set; }
        public virtual Guid IncidentId { get; private set; }
        public virtual Incident Incident { get; private set; }
        public virtual Guid? AssignedPersonId { get; private set; }
        public virtual Person AssignedPerson { get; private set; }
        public virtual string AssignedTeamName { get; private set; }
        public virtual long? AssignedByUserId { get; private set; }
        public virtual DateTime AssignedAt { get; private set; }
        public virtual DateTime? UnassignedAt { get; private set; }
        public virtual string DispatchNotes { get; private set; }
        public virtual bool IsActive { get; private set; }

        public static IncidentAssignment Create(
            int tenantId,
            Incident incident,
            Person assignee,
            long assignedByUserId,
            string teamName = null,
            string dispatchNotes = null)
        {
            return new IncidentAssignment
            {
                TenantId = tenantId,
                Incident = incident,
                IncidentId = incident.Id,
                AssignedPerson = assignee,
                AssignedPersonId = assignee?.Id,
                AssignedTeamName = string.IsNullOrWhiteSpace(teamName) ? null : teamName.Trim(),
                AssignedByUserId = assignedByUserId,
                AssignedAt = DateTime.UtcNow,
                DispatchNotes = dispatchNotes?.Trim(),
                IsActive = true
            };
        }

        public virtual void Deactivate()
        {
            IsActive = false;
            UnassignedAt = DateTime.UtcNow;
        }
    }
}
