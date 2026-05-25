using Abp.Domain.Entities;
using Abp.Domain.Entities.Auditing;
using System;

namespace Farmru.IotMonitoring.Domains.Nodes
{
    /// <summary>
    /// Child entity of the <see cref="Node"/> aggregate.
    /// </summary>
    public class NodeReplacementHistory : CreationAuditedEntity<Guid>, IMustHaveTenant
    {
        protected NodeReplacementHistory()
        {
        }

        public virtual int TenantId { get; set; }
        public virtual Node Node { get; private set; }
        public virtual string OldSerialNumber { get; private set; }
        public virtual string NewSerialNumber { get; private set; }
        public virtual DateTime ReplacedAt { get; private set; }
        public virtual string Reason { get; private set; }
        public virtual string Notes { get; private set; }

        internal static NodeReplacementHistory Record(
            int tenantId,
            Node node,
            string oldSerialNumber,
            string newSerialNumber,
            string reason,
            string notes)
        {
            return new NodeReplacementHistory
            {
                Id = Guid.NewGuid(),
                TenantId = tenantId,
                Node = node,
                OldSerialNumber = oldSerialNumber,
                NewSerialNumber = newSerialNumber,
                ReplacedAt = DateTime.UtcNow,
                Reason = reason,
                Notes = notes
            };
        }
    }
}
