using Abp.Application.Services.Dto;
using Farmru.IotMonitoring.Domains.Alerts;
using Farmru.IotMonitoring.Domains.Weather;
using Farmru.IotMonitoring.Helpers;
using System;
using System.ComponentModel.DataAnnotations;

namespace Farmru.IotMonitoring.Services.Weather.Dto
{
    public class WeatherObservationDto
    {
        public Guid Id { get; set; }
        public Guid FacilityId { get; set; }
        public DateTime ObservedAt { get; set; }
        public decimal TemperatureCelsius { get; set; }
        public decimal HumidityPercent { get; set; }
        public decimal? WindSpeedKph { get; set; }
        public int? WindDirectionDegrees { get; set; }
        public decimal? PrecipitationMm { get; set; }
        public decimal? PressureHpa { get; set; }
        public decimal? UvIndex { get; set; }
        public decimal? LightningProbabilityPercent { get; set; }
    }

    public class WeatherForecastDto
    {
        public Guid Id { get; set; }
        public Guid FacilityId { get; set; }
        public DateTime ForecastFor { get; set; }
        public DateTime GeneratedAt { get; set; }
        public decimal TempMinCelsius { get; set; }
        public decimal TempMaxCelsius { get; set; }
        public int PrecipitationProbabilityPercent { get; set; }
        public decimal? WindGustKph { get; set; }
        public FrostRiskLevel FrostRisk { get; set; }
        public HeatStressLevel HeatStress { get; set; }
    }

    public class EvapotranspirationDto
    {
        public Guid Id { get; set; }
        public Guid FacilityId { get; set; }
        public DateTime Date { get; set; }
        public decimal Et0Mm { get; set; }
        public decimal? EtcMm { get; set; }
        public Guid? CropSeasonId { get; set; }
    }

    public class GetWeatherHistoryInput : PagedResultRequestDto
    {
        [Required]
        public Guid FacilityId { get; set; }
        public DateTime? FromDate { get; set; }
        public DateTime? ToDate { get; set; }
    }

    public class GetEvapotranspirationInput
    {
        [Required]
        public Guid FacilityId { get; set; }
        public DateTime? FromDate { get; set; }
        public DateTime? ToDate { get; set; }
    }

    public class WeatherAlertRuleDto
    {
        public Guid Id { get; set; }
        public EntityWithDisplayNameDto<Guid?> Facility { get; set; }
        public EntityWithDisplayNameDto<Guid?> Organisation { get; set; }
        public WeatherAlertType AlertType { get; set; }
        public decimal ThresholdValue { get; set; }
        public AlertSeverity Severity { get; set; }
        public bool IsActive { get; set; }
    }

    public class CreateWeatherAlertRuleInput
    {
        public Guid? FacilityId { get; set; }
        public Guid? OrganisationId { get; set; }

        [Required]
        public WeatherAlertType AlertType { get; set; }

        [Required]
        public decimal ThresholdValue { get; set; }

        [Required]
        public AlertSeverity Severity { get; set; }
    }
}
