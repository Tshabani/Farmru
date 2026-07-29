using Abp.Domain.Entities;
using Abp.Domain.Entities.Auditing;
using Farmru.IotMonitoring.Domains;
using System;
using System.Collections.Generic;

namespace Farmru.IotMonitoring.Domains.Crops
{
    /// <summary>
    /// Aggregate root for a single planting cycle on a Field. Owns GrowthStageEvent history
    /// and the (at most one) HarvestRecord as part of its consistency boundary — a season's
    /// stage history and harvest outcome are never meaningful independent of the season
    /// itself (Phase 1 Technical Design Section 2.2).
    /// </summary>
    public class CropSeason : FullAuditedAggregateRoot<Guid>, IMustHaveTenant
    {
        private readonly List<GrowthStageEvent> _stageEvents = new();

        protected CropSeason()
        {
        }

        public int TenantId { get; set; }
        public virtual Guid FieldId { get; private set; }
        public virtual Field Field { get; private set; }
        public virtual Guid CropTypeId { get; private set; }
        public virtual CropType CropType { get; private set; }
        public virtual Guid? SeedVarietyId { get; private set; }
        public virtual SeedVariety SeedVariety { get; private set; }
        public virtual DateTime PlantingDate { get; private set; }
        public virtual DateTime ExpectedHarvestDate { get; private set; }
        public virtual decimal? ExpectedYieldKg { get; private set; }
        public virtual int? PlantPopulationPerHectare { get; private set; }
        public virtual CropSeasonStatus Status { get; private set; }
        public virtual HarvestRecord Harvest { get; private set; }

        public virtual IReadOnlyCollection<GrowthStageEvent> StageEvents => _stageEvents.AsReadOnly();

        public static CropSeason Plant(
            int tenantId,
            Field field,
            CropType cropType,
            SeedVariety seedVariety,
            DateTime plantingDate,
            DateTime expectedHarvestDate,
            decimal? expectedYieldKg = null,
            int? plantPopulationPerHectare = null)
        {
            if (field == null)
            {
                throw new DomainRuleException("A Crop Season requires a Field.");
            }

            if (cropType == null)
            {
                throw new DomainRuleException("A Crop Season requires a Crop Type.");
            }

            if (expectedHarvestDate.Date <= plantingDate.Date)
            {
                throw new DomainRuleException("Expected harvest date must be after planting date.");
            }

            if (expectedYieldKg.HasValue && expectedYieldKg.Value < 0)
            {
                throw new DomainRuleException("Expected yield cannot be negative.");
            }

            if (plantPopulationPerHectare.HasValue && plantPopulationPerHectare.Value <= 0)
            {
                throw new DomainRuleException("Plant population must be greater than zero.");
            }

            var season = new CropSeason
            {
                TenantId = tenantId,
                Field = field,
                FieldId = field.Id,
                CropType = cropType,
                CropTypeId = cropType.Id,
                SeedVariety = seedVariety,
                SeedVarietyId = seedVariety?.Id,
                PlantingDate = plantingDate.Date,
                ExpectedHarvestDate = expectedHarvestDate.Date,
                ExpectedYieldKg = expectedYieldKg,
                PlantPopulationPerHectare = plantPopulationPerHectare,
                Status = CropSeasonStatus.Planned
            };
            season._stageEvents.Add(GrowthStageEvent.Create(season, GrowthStage.Planted, plantingDate, GrowthStageSource.Manual));
            return season;
        }

        public virtual void LogGrowthStage(GrowthStage stage, DateTime observedDate, GrowthStageSource source)
        {
            if (Status == CropSeasonStatus.Closed)
            {
                throw new DomainRuleException("Cannot log a growth stage on a closed season.");
            }

            if (Status == CropSeasonStatus.Harvested)
            {
                throw new DomainRuleException("Cannot log a growth stage on an already-harvested season.");
            }

            _stageEvents.Add(GrowthStageEvent.Create(this, stage, observedDate, source));
            if (Status == CropSeasonStatus.Planned)
            {
                Status = CropSeasonStatus.Growing;
            }
        }

        public virtual HarvestRecord RecordHarvest(DateTime harvestDate, decimal actualYieldKg, string qualityGrade)
        {
            if (Status != CropSeasonStatus.Growing)
            {
                throw new DomainRuleException($"Cannot harvest a season in status {Status}. Expected: Growing.");
            }

            var record = HarvestRecord.Create(this, harvestDate, actualYieldKg, qualityGrade);
            Harvest = record;
            Status = CropSeasonStatus.Harvested;
            _stageEvents.Add(GrowthStageEvent.Create(this, GrowthStage.Harvested, harvestDate, GrowthStageSource.Manual));
            return record;
        }

        public virtual void Close()
        {
            if (Status != CropSeasonStatus.Harvested)
            {
                throw new DomainRuleException("Only a harvested season can be closed.");
            }

            Status = CropSeasonStatus.Closed;
        }
    }
}
