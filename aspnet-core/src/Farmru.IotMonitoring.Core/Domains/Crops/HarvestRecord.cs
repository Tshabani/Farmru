using Abp.Domain.Entities;
using Abp.Domain.Entities.Auditing;
using Farmru.IotMonitoring.Domains;
using System;

namespace Farmru.IotMonitoring.Domains.Crops
{
    public class HarvestRecord : FullAuditedAggregateRoot<Guid>, IMustHaveTenant
    {
        protected HarvestRecord()
        {
        }

        public int TenantId { get; set; }
        public virtual Guid CropSeasonId { get; private set; }
        public virtual CropSeason CropSeason { get; private set; }
        public virtual DateTime HarvestDate { get; private set; }
        public virtual decimal ActualYieldKg { get; private set; }
        public virtual string QualityGrade { get; private set; }

        internal static HarvestRecord Create(CropSeason cropSeason, DateTime harvestDate, decimal actualYieldKg, string qualityGrade)
        {
            if (cropSeason == null)
            {
                throw new DomainRuleException("A harvest record must belong to a Crop Season.");
            }

            if (actualYieldKg < 0)
            {
                throw new DomainRuleException("Actual yield cannot be negative.");
            }

            return new HarvestRecord
            {
                TenantId = cropSeason.TenantId,
                CropSeason = cropSeason,
                CropSeasonId = cropSeason.Id,
                HarvestDate = harvestDate.Date,
                ActualYieldKg = actualYieldKg,
                QualityGrade = string.IsNullOrWhiteSpace(qualityGrade) ? null : qualityGrade.Trim()
            };
        }
    }
}
