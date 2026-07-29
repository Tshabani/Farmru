using Abp.Domain.Entities;
using Abp.Domain.Entities.Auditing;
using Farmru.IotMonitoring.Domains;
using System;

namespace Farmru.IotMonitoring.Domains.Crops
{
    public class SeedVariety : FullAuditedAggregateRoot<Guid>, IMustHaveTenant
    {
        protected SeedVariety()
        {
        }

        public int TenantId { get; set; }
        public virtual Guid CropTypeId { get; private set; }
        public virtual CropType CropType { get; private set; }
        public virtual Guid? SupplierId { get; private set; }
        public virtual SeedSupplier Supplier { get; private set; }
        public virtual string Name { get; private set; }
        public virtual int? DaysToMaturity { get; private set; }

        public static SeedVariety Create(
            int tenantId,
            CropType cropType,
            string name,
            SeedSupplier supplier = null,
            int? daysToMaturity = null)
        {
            if (cropType == null)
            {
                throw new DomainRuleException("A Seed Variety must belong to a Crop Type.");
            }

            if (daysToMaturity.HasValue && daysToMaturity.Value <= 0)
            {
                throw new DomainRuleException("Days to maturity must be greater than zero.");
            }

            var trimmed = name?.Trim();
            if (string.IsNullOrWhiteSpace(trimmed))
            {
                throw new DomainRuleException("Seed variety name is required.");
            }

            return new SeedVariety
            {
                TenantId = tenantId,
                CropType = cropType,
                CropTypeId = cropType.Id,
                Supplier = supplier,
                SupplierId = supplier?.Id,
                Name = trimmed,
                DaysToMaturity = daysToMaturity
            };
        }
    }
}
