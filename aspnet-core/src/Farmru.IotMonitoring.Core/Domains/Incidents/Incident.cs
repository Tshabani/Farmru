using Abp.Domain.Entities.Auditing;
using Farmru.IotMonitoring.Authorization.Users;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Farmru.IotMonitoring.Domains.Incidents
{
    public class Incident : FullAuditedEntity<Guid>
    { 
        public virtual string? Title { get; set; }
        public virtual string? Description { get; set; }
        public virtual IncidentStatus? Status { get; set; }
        public virtual IncidentPriority? Priority { get; set; }
        public virtual DateTime CreatedDate { get; set; }
        public virtual DateTime? ResolvedDate { get; set; }
        public virtual User? CreatedBy { get; set; }
        public virtual User? AssignedTo { get; set; }
    }

    public enum IncidentStatus
    {
        Open,
        InProgress,
        Resolved,
        Closed
    }

    public enum IncidentPriority
    {
        Low,
        Medium,
        High,
        Critical
    }
}
