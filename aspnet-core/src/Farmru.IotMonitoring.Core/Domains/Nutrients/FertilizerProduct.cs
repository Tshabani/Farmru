using Abp.Domain.Entities;
using Abp.Domain.Entities.Auditing;
using Farmru.IotMonitoring.Domains;
using System;

namespace Farmru.IotMonitoring.Domains.Nutrients
{
    public class FertilizerProduct : FullAuditedAggregateRoot<Guid>, IMustHaveTenant
    {
        protected FertilizerProduct()
        {
        }

        public int TenantId { get; set; }
        public virtual string Name { get; private set; }
        public virtual decimal NitrogenPercent { get; private set; }
        public virtual decimal PhosphorusPercent { get; private set; }
        public virtual decimal PotassiumPercent { get; private set; }
        public virtual decimal? UnitCostPerKg { get; private set; }

        // Unconstrained seam: no Supplier concept exists for fertilizer products yet
        // (Marketplace/Supplier is Phase 5) — see Technical Design Section 3.4.
        public virtual Guid? SupplierId { get; private set; }

        public static FertilizerProduct Create(
            int tenantId,
            string name,
            decimal nitrogenPercent,
            decimal phosphorusPercent,
            decimal potassiumPercent,
            decimal? unitCostPerKg = null,
            Guid? supplierId = null)
        {
            var product = new FertilizerProduct { TenantId = tenantId, SupplierId = supplierId };
            product.UpdateDetails(name, nitrogenPercent, phosphorusPercent, potassiumPercent, unitCostPerKg);
            return product;
        }

        public virtual void UpdateDetails(
            string name,
            decimal nitrogenPercent,
            decimal phosphorusPercent,
            decimal potassiumPercent,
            decimal? unitCostPerKg)
        {
            var trimmed = name?.Trim();
            if (string.IsNullOrWhiteSpace(trimmed))
            {
                throw new DomainRuleException("Fertilizer product name is required.");
            }

            if (nitrogenPercent < 0 || nitrogenPercent > 100
                || phosphorusPercent < 0 || phosphorusPercent > 100
                || potassiumPercent < 0 || potassiumPercent > 100)
            {
                throw new DomainRuleException("Nutrient composition percentages must be between 0 and 100.");
            }

            if (unitCostPerKg.HasValue && unitCostPerKg.Value < 0)
            {
                throw new DomainRuleException("Unit cost cannot be negative.");
            }

            Name = trimmed;
            NitrogenPercent = nitrogenPercent;
            PhosphorusPercent = phosphorusPercent;
            PotassiumPercent = potassiumPercent;
            UnitCostPerKg = unitCostPerKg;
        }
    }
}
