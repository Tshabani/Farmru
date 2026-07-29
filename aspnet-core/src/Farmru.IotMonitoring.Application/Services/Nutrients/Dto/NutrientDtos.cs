using Abp.Application.Services.Dto;
using Farmru.IotMonitoring.Domains.Nutrients;
using Farmru.IotMonitoring.Helpers;
using System;
using System.ComponentModel.DataAnnotations;

namespace Farmru.IotMonitoring.Services.Nutrients.Dto
{
    public class FertilizerProductDto : EntityDto<Guid>
    {
        public string Name { get; set; }
        public decimal NitrogenPercent { get; set; }
        public decimal PhosphorusPercent { get; set; }
        public decimal PotassiumPercent { get; set; }
        public decimal? UnitCostPerKg { get; set; }
    }

    public class CreateFertilizerProductInput
    {
        [Required]
        [StringLength(100, MinimumLength = 2)]
        public string Name { get; set; }

        [Required]
        [Range(0, 100)]
        public decimal NitrogenPercent { get; set; }

        [Required]
        [Range(0, 100)]
        public decimal PhosphorusPercent { get; set; }

        [Required]
        [Range(0, 100)]
        public decimal PotassiumPercent { get; set; }

        public decimal? UnitCostPerKg { get; set; }
    }

    public class FertilizerApplicationDto : EntityDto<Guid>
    {
        public EntityWithDisplayNameDto<Guid?> Field { get; set; }
        public EntityWithDisplayNameDto<Guid?> CropSeason { get; set; }
        public EntityWithDisplayNameDto<Guid?> Product { get; set; }
        public decimal RateKgPerHectare { get; set; }
        public DateTime ApplicationDate { get; set; }
        public decimal? Cost { get; set; }
        public EntityWithDisplayNameDto<Guid?> Operator { get; set; }
    }

    public class RecordFertilizerApplicationInput
    {
        [Required]
        public Guid FieldId { get; set; }

        public Guid? CropSeasonId { get; set; }

        [Required]
        public Guid ProductId { get; set; }

        [Required]
        [Range(0.01, double.MaxValue)]
        public decimal RateKgPerHectare { get; set; }

        [Required]
        public DateTime ApplicationDate { get; set; }

        public decimal? Cost { get; set; }
        public Guid? OperatorPersonId { get; set; }
    }

    public class GetApplicationsByFieldInput : PagedResultRequestDto
    {
        [Required]
        public Guid FieldId { get; set; }
    }

    public class NutrientBalanceSnapshotDto
    {
        public Guid Id { get; set; }
        public Guid FieldId { get; set; }
        public DateTime SnapshotDate { get; set; }
        public decimal SensedNitrogen { get; set; }
        public decimal SensedPhosphorus { get; set; }
        public decimal SensedPotassium { get; set; }
        public decimal AppliedNitrogenTrailing30d { get; set; }
        public decimal AppliedPhosphorusTrailing30d { get; set; }
        public decimal AppliedPotassiumTrailing30d { get; set; }
        public NutrientBalanceStatus NitrogenStatus { get; set; }
        public NutrientBalanceStatus PhosphorusStatus { get; set; }
        public NutrientBalanceStatus PotassiumStatus { get; set; }
    }

    public class GetNutrientHistoryInput
    {
        [Required]
        public Guid FieldId { get; set; }
        public DateTime? FromDate { get; set; }
        public DateTime? ToDate { get; set; }
    }
}
