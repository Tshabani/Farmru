using Abp.Domain.Entities;
using Abp.Domain.Entities.Auditing;
using Farmru.IotMonitoring.Domains;
using System;

namespace Farmru.IotMonitoring.Domains.Crops
{
    public class SeedSupplier : FullAuditedAggregateRoot<Guid>, IMustHaveTenant
    {
        protected SeedSupplier()
        {
        }

        public int TenantId { get; set; }
        public virtual string Name { get; private set; }
        public virtual string ContactInfo { get; private set; }

        public static SeedSupplier Create(int tenantId, string name, string contactInfo = null)
        {
            var supplier = new SeedSupplier { TenantId = tenantId };
            supplier.UpdateDetails(name, contactInfo);
            return supplier;
        }

        public virtual void UpdateDetails(string name, string contactInfo)
        {
            var trimmed = name?.Trim();
            if (string.IsNullOrWhiteSpace(trimmed))
            {
                throw new DomainRuleException("Seed supplier name is required.");
            }

            Name = trimmed;
            ContactInfo = string.IsNullOrWhiteSpace(contactInfo) ? null : contactInfo.Trim();
        }
    }
}
