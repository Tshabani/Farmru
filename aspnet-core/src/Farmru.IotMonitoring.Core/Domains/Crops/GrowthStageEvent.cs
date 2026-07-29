using Abp.Domain.Entities;
using Abp.Domain.Entities.Auditing;
using Farmru.IotMonitoring.Domains;
using System;

namespace Farmru.IotMonitoring.Domains.Crops
{
    public class GrowthStageEvent : FullAuditedAggregateRoot<Guid>, IMustHaveTenant
    {
        protected GrowthStageEvent()
        {
        }

        public int TenantId { get; set; }
        public virtual Guid CropSeasonId { get; private set; }
        public virtual CropSeason CropSeason { get; private set; }
        public virtual GrowthStage Stage { get; private set; }
        public virtual DateTime ObservedDate { get; private set; }
        public virtual GrowthStageSource Source { get; private set; }

        internal static GrowthStageEvent Create(CropSeason cropSeason, GrowthStage stage, DateTime observedDate, GrowthStageSource source)
        {
            if (cropSeason == null)
            {
                throw new DomainRuleException("A growth stage event must belong to a Crop Season.");
            }

            return new GrowthStageEvent
            {
                TenantId = cropSeason.TenantId,
                CropSeason = cropSeason,
                CropSeasonId = cropSeason.Id,
                Stage = stage,
                ObservedDate = observedDate.Date,
                Source = source
            };
        }
    }
}
