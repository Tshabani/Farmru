using Abp.Domain.Entities;
using Abp.Domain.Entities.Auditing;
using Farmru.IotMonitoring.Domains;
using System;

namespace Farmru.IotMonitoring.Domains.Incidents
{
    public class IncidentAttachment : FullAuditedEntity<Guid>, IMustHaveTenant
    {
        protected IncidentAttachment()
        {
        }

        public int TenantId { get; set; }
        public virtual Guid IncidentId { get; private set; }
        public virtual Incident Incident { get; private set; }
        public virtual string FileName { get; private set; }
        public virtual string ContentType { get; private set; }
        public virtual string StoragePath { get; private set; }
        public virtual long FileSizeBytes { get; private set; }
        public virtual string Caption { get; private set; }

        public static IncidentAttachment Create(
            int tenantId,
            Incident incident,
            string fileName,
            string contentType,
            string storagePath,
            long fileSizeBytes,
            string caption = null)
        {
            if (string.IsNullOrWhiteSpace(fileName))
            {
                throw new DomainRuleException("File name is required.");
            }

            return new IncidentAttachment
            {
                TenantId = tenantId,
                Incident = incident,
                IncidentId = incident.Id,
                FileName = fileName.Trim(),
                ContentType = contentType ?? "application/octet-stream",
                StoragePath = storagePath,
                FileSizeBytes = fileSizeBytes,
                Caption = caption?.Trim()
            };
        }
    }
}
