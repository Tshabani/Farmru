using Abp.Domain.Entities;
using Abp.Domain.Entities.Auditing;
using Farmru.IotMonitoring.Domains;
using Farmru.IotMonitoring.Domains.Crops;
using Farmru.IotMonitoring.Domains.Persons;
using System;

namespace Farmru.IotMonitoring.Domains.Nutrients
{
    public class FertilizerApplication : FullAuditedAggregateRoot<Guid>, IMustHaveTenant
    {
        protected FertilizerApplication()
        {
        }

        public int TenantId { get; set; }
        public virtual Guid FieldId { get; private set; }
        public virtual Field Field { get; private set; }
        public virtual Guid? CropSeasonId { get; private set; }
        public virtual CropSeason CropSeason { get; private set; }
        public virtual Guid ProductId { get; private set; }
        public virtual FertilizerProduct Product { get; private set; }
        public virtual decimal RateKgPerHectare { get; private set; }
        public virtual DateTime ApplicationDate { get; private set; }
        public virtual decimal? Cost { get; private set; }
        public virtual Guid? OperatorPersonId { get; private set; }
        public virtual Person Operator { get; private set; }

        // Unconstrained seam: Inventory Management (Phase 3) is what will populate this —
        // see Technical Design Section 3.4.
        public virtual Guid? InventorySourceRef { get; private set; }

        public static FertilizerApplication Apply(
            int tenantId,
            Field field,
            CropSeason cropSeason,
            FertilizerProduct product,
            decimal rateKgPerHectare,
            DateTime applicationDate,
            decimal? cost = null,
            Person appliedBy = null)
        {
            if (field == null)
            {
                throw new DomainRuleException("A fertilizer application requires a Field.");
            }

            if (product == null)
            {
                throw new DomainRuleException("A fertilizer application requires a Fertilizer Product.");
            }

            if (rateKgPerHectare <= 0)
            {
                throw new DomainRuleException("Application rate must be positive.");
            }

            if (cost.HasValue && cost.Value < 0)
            {
                throw new DomainRuleException("Cost cannot be negative.");
            }

            if (cropSeason != null && cropSeason.FieldId != field.Id)
            {
                throw new DomainRuleException("The Crop Season must belong to the same Field as the application.");
            }

            return new FertilizerApplication
            {
                TenantId = tenantId,
                Field = field,
                FieldId = field.Id,
                CropSeason = cropSeason,
                CropSeasonId = cropSeason?.Id,
                Product = product,
                ProductId = product.Id,
                RateKgPerHectare = rateKgPerHectare,
                ApplicationDate = applicationDate.Date,
                Cost = cost,
                Operator = appliedBy,
                OperatorPersonId = appliedBy?.Id
            };
        }
    }
}
