using Abp.Application.Services.Dto;
using Abp.Authorization;
using Abp.Domain.Repositories;
using Abp.Linq.Extensions;
using Abp.Runtime.Session;
using Abp.UI;
using Farmru.IotMonitoring.Authorization;
using Farmru.IotMonitoring.Domains;
using Farmru.IotMonitoring.Domains.Crops;
using Farmru.IotMonitoring.Helpers;
using Farmru.IotMonitoring.Services.Crops.Dto;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Farmru.IotMonitoring.Services.Crops
{
    [AbpAuthorize(PermissionNames.Pages_Crops)]
    public class CropSeasonAppService : IotMonitoringAppServiceBase, ICropSeasonAppService
    {
        private readonly IRepository<CropSeason, Guid> _seasonRepository;
        private readonly IRepository<Field, Guid> _fieldRepository;
        private readonly IRepository<CropType, Guid> _cropTypeRepository;
        private readonly IRepository<SeedVariety, Guid> _seedVarietyRepository;

        public CropSeasonAppService(
            IRepository<CropSeason, Guid> seasonRepository,
            IRepository<Field, Guid> fieldRepository,
            IRepository<CropType, Guid> cropTypeRepository,
            IRepository<SeedVariety, Guid> seedVarietyRepository)
        {
            _seasonRepository = seasonRepository;
            _fieldRepository = fieldRepository;
            _cropTypeRepository = cropTypeRepository;
            _seedVarietyRepository = seedVarietyRepository;
        }

        [AbpAuthorize(PermissionNames.Pages_Crops_Manage)]
        public async Task<CropSeasonDto> Plant(PlantCropSeasonInput input)
        {
            var field = await _fieldRepository.FirstOrDefaultAsync(input.FieldId)
                ?? throw new UserFriendlyException(L("FieldNotFound"));

            var cropType = await _cropTypeRepository.FirstOrDefaultAsync(input.CropTypeId)
                ?? throw new UserFriendlyException(L("CropTypeNotFound"));

            SeedVariety seedVariety = null;
            if (input.SeedVarietyId.HasValue)
            {
                seedVariety = await _seedVarietyRepository.FirstOrDefaultAsync(input.SeedVarietyId.Value)
                    ?? throw new UserFriendlyException(L("SeedVarietyNotFound"));
            }

            // Cross-aggregate invariant (ADR-008 Layer 4): a Field cannot have two open
            // seasons at once. A single CropSeason aggregate can't see its siblings, so this
            // check lives here, before calling the domain factory — matching the existing
            // INodeSerialNumberAvailabilityChecker pattern for the analogous Node invariant.
            var hasOpenSeason = await _seasonRepository.GetAll()
                .AnyAsync(s => s.FieldId == input.FieldId
                    && (s.Status == CropSeasonStatus.Planned || s.Status == CropSeasonStatus.Growing));

            if (hasOpenSeason)
            {
                throw new UserFriendlyException(L("FieldAlreadyHasOpenSeason"));
            }

            try
            {
                var season = CropSeason.Plant(
                    AbpSession.GetTenantId(),
                    field,
                    cropType,
                    seedVariety,
                    input.PlantingDate,
                    input.ExpectedHarvestDate,
                    input.ExpectedYieldKg,
                    input.PlantPopulationPerHectare);

                await _seasonRepository.InsertAsync(season);
                await CurrentUnitOfWork.SaveChangesAsync();
                return MapToDto(season);
            }
            catch (DomainRuleException ex)
            {
                throw new UserFriendlyException(ex.Message);
            }
        }

        public async Task<CropSeasonDetailDto> GetDetail(EntityDto<Guid> input)
        {
            var season = await GetSeasonOrThrowAsync(input.Id, includeStageEvents: true);
            return MapToDetailDto(season);
        }

        public async Task<PagedResultDto<CropSeasonDto>> GetByField(GetCropSeasonsByFieldInput input)
        {
            var query = _seasonRepository.GetAll()
                .Include(s => s.Field)
                .Include(s => s.CropType)
                .Include(s => s.SeedVariety)
                .Where(s => s.FieldId == input.FieldId);

            var totalCount = await query.CountAsync();
            var items = await query
                .OrderByDescending(s => s.PlantingDate)
                .PageBy(input)
                .ToListAsync();

            return new PagedResultDto<CropSeasonDto>(totalCount, items.Select(MapToDto).ToList());
        }

        [AbpAuthorize(PermissionNames.Pages_Crops_Manage)]
        public async Task<CropSeasonDetailDto> LogGrowthStage(LogGrowthStageInput input)
        {
            var season = await GetSeasonOrThrowAsync(input.CropSeasonId, includeStageEvents: true);
            try
            {
                season.LogGrowthStage(input.Stage, input.ObservedDate, GrowthStageSource.Manual);
                await _seasonRepository.UpdateAsync(season);
                await CurrentUnitOfWork.SaveChangesAsync();
                return MapToDetailDto(season);
            }
            catch (DomainRuleException ex)
            {
                throw new UserFriendlyException(ex.Message);
            }
        }

        [AbpAuthorize(PermissionNames.Pages_Crops_Harvest)]
        public async Task<CropSeasonDetailDto> Harvest(HarvestCropSeasonInput input)
        {
            var season = await GetSeasonOrThrowAsync(input.CropSeasonId, includeStageEvents: true);
            try
            {
                season.RecordHarvest(input.HarvestDate, input.ActualYieldKg, input.QualityGrade);
                await _seasonRepository.UpdateAsync(season);
                await CurrentUnitOfWork.SaveChangesAsync();
                return MapToDetailDto(season);
            }
            catch (DomainRuleException ex)
            {
                throw new UserFriendlyException(ex.Message);
            }
        }

        [AbpAuthorize(PermissionNames.Pages_Crops_Manage)]
        public async Task<CropSeasonDto> Close(EntityDto<Guid> input)
        {
            var season = await GetSeasonOrThrowAsync(input.Id, includeStageEvents: false);
            try
            {
                season.Close();
                await _seasonRepository.UpdateAsync(season);
                await CurrentUnitOfWork.SaveChangesAsync();
                return MapToDto(season);
            }
            catch (DomainRuleException ex)
            {
                throw new UserFriendlyException(ex.Message);
            }
        }

        public async Task<List<CropRotationEntryDto>> GetRotationHistory(EntityDto<Guid> fieldId)
        {
            // Computed query, not a stored entity (Technical Design Section 2.2 note) — avoids
            // a denormalized table that could drift from the source CropSeason records.
            var seasons = await _seasonRepository.GetAll()
                .Include(s => s.CropType)
                .Include(s => s.Harvest)
                .Where(s => s.FieldId == fieldId.Id && s.Status == CropSeasonStatus.Closed)
                .OrderByDescending(s => s.PlantingDate)
                .ToListAsync();

            return seasons.Select(s => new CropRotationEntryDto
            {
                CropSeasonId = s.Id,
                CropTypeName = s.CropType?.Name,
                PlantingDate = s.PlantingDate,
                HarvestDate = s.Harvest?.HarvestDate,
                ActualYieldKg = s.Harvest?.ActualYieldKg
            }).ToList();
        }

        private async Task<CropSeason> GetSeasonOrThrowAsync(Guid id, bool includeStageEvents)
        {
            var query = _seasonRepository.GetAll()
                .Include(s => s.Field)
                .Include(s => s.CropType)
                .Include(s => s.SeedVariety)
                .Include(s => s.Harvest)
                .AsQueryable();

            if (includeStageEvents)
            {
                query = query.Include(s => s.StageEvents);
            }

            var season = await query.FirstOrDefaultAsync(s => s.Id == id);
            if (season == null)
            {
                throw new UserFriendlyException(L("CropSeasonNotFound"));
            }

            return season;
        }

        private static CropSeasonDto MapToDto(CropSeason season) => new CropSeasonDto
        {
            Id = season.Id,
            Field = season.Field != null
                ? new EntityWithDisplayNameDto<Guid?> { Id = season.Field.Id, DisplayText = season.Field.Name }
                : null,
            CropType = season.CropType != null
                ? new EntityWithDisplayNameDto<Guid?> { Id = season.CropType.Id, DisplayText = season.CropType.Name }
                : null,
            SeedVariety = season.SeedVariety != null
                ? new EntityWithDisplayNameDto<Guid?> { Id = season.SeedVariety.Id, DisplayText = season.SeedVariety.Name }
                : null,
            PlantingDate = season.PlantingDate,
            ExpectedHarvestDate = season.ExpectedHarvestDate,
            ExpectedYieldKg = season.ExpectedYieldKg,
            PlantPopulationPerHectare = season.PlantPopulationPerHectare,
            Status = season.Status
        };

        private static CropSeasonDetailDto MapToDetailDto(CropSeason season)
        {
            var dto = MapToDto(season);
            var detail = new CropSeasonDetailDto
            {
                Id = dto.Id,
                Field = dto.Field,
                CropType = dto.CropType,
                SeedVariety = dto.SeedVariety,
                PlantingDate = dto.PlantingDate,
                ExpectedHarvestDate = dto.ExpectedHarvestDate,
                ExpectedYieldKg = dto.ExpectedYieldKg,
                PlantPopulationPerHectare = dto.PlantPopulationPerHectare,
                Status = dto.Status,
                StageEvents = season.StageEvents
                    .OrderBy(e => e.ObservedDate)
                    .Select(e => new GrowthStageEventDto { Id = e.Id, Stage = e.Stage, ObservedDate = e.ObservedDate, Source = e.Source })
                    .ToList(),
                Harvest = season.Harvest != null
                    ? new HarvestRecordDto
                    {
                        Id = season.Harvest.Id,
                        HarvestDate = season.Harvest.HarvestDate,
                        ActualYieldKg = season.Harvest.ActualYieldKg,
                        QualityGrade = season.Harvest.QualityGrade
                    }
                    : null
            };
            return detail;
        }
    }
}
