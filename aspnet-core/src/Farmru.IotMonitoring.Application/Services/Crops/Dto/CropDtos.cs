using Abp.Application.Services.Dto;
using Abp.AutoMapper;
using Farmru.IotMonitoring.Domains.Crops;
using Farmru.IotMonitoring.Helpers;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace Farmru.IotMonitoring.Services.Crops.Dto
{
    public class FieldDto : EntityDto<Guid>
    {
        public EntityWithDisplayNameDto<Guid?> Facility { get; set; }
        public string Name { get; set; }
        public decimal? AreaHectares { get; set; }
        public string SoilType { get; set; }
        public Guid? BoundaryGeoFenceId { get; set; }
    }

    public class CreateFieldInput
    {
        [Required]
        public Guid FacilityId { get; set; }

        [Required]
        [StringLength(100, MinimumLength = 2)]
        public string Name { get; set; }

        public decimal? AreaHectares { get; set; }
        public string SoilType { get; set; }
        public Guid? BoundaryGeoFenceId { get; set; }
    }

    public class PagedFieldResultRequestDto : PagedResultRequestDto
    {
        public Guid? FacilityId { get; set; }
    }

    [AutoMap(typeof(CropType))]
    public class CropTypeDto : EntityDto<Guid>
    {
        public string Name { get; set; }
        public string ScientificName { get; set; }
        public int TypicalGrowthDurationDays { get; set; }
        public bool IsActive { get; set; }
    }

    public class CreateCropTypeInput
    {
        [Required]
        [StringLength(100, MinimumLength = 2)]
        public string Name { get; set; }

        public string ScientificName { get; set; }

        [Required]
        [Range(1, 1000)]
        public int TypicalGrowthDurationDays { get; set; }
    }

    [AutoMap(typeof(SeedSupplier))]
    public class SeedSupplierDto : EntityDto<Guid>
    {
        public string Name { get; set; }
        public string ContactInfo { get; set; }
    }

    public class CreateSeedSupplierInput
    {
        [Required]
        [StringLength(100, MinimumLength = 2)]
        public string Name { get; set; }

        public string ContactInfo { get; set; }
    }

    public class SeedVarietyDto : EntityDto<Guid>
    {
        public EntityWithDisplayNameDto<Guid?> CropType { get; set; }
        public EntityWithDisplayNameDto<Guid?> Supplier { get; set; }
        public string Name { get; set; }
        public int? DaysToMaturity { get; set; }
    }

    public class CreateSeedVarietyInput
    {
        [Required]
        public Guid CropTypeId { get; set; }

        public Guid? SupplierId { get; set; }

        [Required]
        [StringLength(100, MinimumLength = 2)]
        public string Name { get; set; }

        public int? DaysToMaturity { get; set; }
    }

    public class GrowthStageEventDto
    {
        public Guid Id { get; set; }
        public GrowthStage Stage { get; set; }
        public DateTime ObservedDate { get; set; }
        public GrowthStageSource Source { get; set; }
    }

    public class HarvestRecordDto
    {
        public Guid Id { get; set; }
        public DateTime HarvestDate { get; set; }
        public decimal ActualYieldKg { get; set; }
        public string QualityGrade { get; set; }
    }

    public class CropSeasonDto : EntityDto<Guid>
    {
        public EntityWithDisplayNameDto<Guid?> Field { get; set; }
        public EntityWithDisplayNameDto<Guid?> CropType { get; set; }
        public EntityWithDisplayNameDto<Guid?> SeedVariety { get; set; }
        public DateTime PlantingDate { get; set; }
        public DateTime ExpectedHarvestDate { get; set; }
        public decimal? ExpectedYieldKg { get; set; }
        public int? PlantPopulationPerHectare { get; set; }
        public CropSeasonStatus Status { get; set; }
    }

    public class CropSeasonDetailDto : CropSeasonDto
    {
        public List<GrowthStageEventDto> StageEvents { get; set; } = new();
        public HarvestRecordDto Harvest { get; set; }
    }

    public class PlantCropSeasonInput
    {
        [Required]
        public Guid FieldId { get; set; }

        [Required]
        public Guid CropTypeId { get; set; }

        public Guid? SeedVarietyId { get; set; }

        [Required]
        public DateTime PlantingDate { get; set; }

        [Required]
        public DateTime ExpectedHarvestDate { get; set; }

        public decimal? ExpectedYieldKg { get; set; }
        public int? PlantPopulationPerHectare { get; set; }
    }

    public class GetCropSeasonsByFieldInput : PagedResultRequestDto
    {
        [Required]
        public Guid FieldId { get; set; }
    }

    public class LogGrowthStageInput
    {
        [Required]
        public Guid CropSeasonId { get; set; }

        [Required]
        public GrowthStage Stage { get; set; }

        [Required]
        public DateTime ObservedDate { get; set; }
    }

    public class HarvestCropSeasonInput
    {
        [Required]
        public Guid CropSeasonId { get; set; }

        [Required]
        public DateTime HarvestDate { get; set; }

        [Required]
        [Range(0, double.MaxValue)]
        public decimal ActualYieldKg { get; set; }

        public string QualityGrade { get; set; }
    }

    public class CropRotationEntryDto
    {
        public Guid CropSeasonId { get; set; }
        public string CropTypeName { get; set; }
        public DateTime PlantingDate { get; set; }
        public DateTime? HarvestDate { get; set; }
        public decimal? ActualYieldKg { get; set; }
    }
}
