using Abp.Domain.Entities;
using Abp.Domain.Entities.Auditing;
using Farmru.IotMonitoring.Domains;
using Farmru.IotMonitoring.Domains.Facilities;
using System;

namespace Farmru.IotMonitoring.Domains.Weather
{
    /// <summary>
    /// A single point-in-time weather reading for a Facility, recorded from an external
    /// weather provider (see Phase 1 Technical Design Section 5.1, IWeatherProvider).
    /// </summary>
    public class WeatherObservation : FullAuditedAggregateRoot<Guid>, IMustHaveTenant
    {
        protected WeatherObservation()
        {
        }

        public int TenantId { get; set; }
        public virtual Guid FacilityId { get; private set; }
        public virtual Facility Facility { get; private set; }
        public virtual DateTime ObservedAt { get; private set; }
        public virtual decimal TemperatureCelsius { get; private set; }
        public virtual decimal HumidityPercent { get; private set; }
        public virtual decimal? WindSpeedKph { get; private set; }
        public virtual int? WindDirectionDegrees { get; private set; }
        public virtual decimal? PrecipitationMm { get; private set; }
        public virtual decimal? PressureHpa { get; private set; }
        public virtual decimal? UvIndex { get; private set; }
        public virtual decimal? LightningProbabilityPercent { get; private set; }
        public virtual string ProviderRef { get; private set; }

        public static WeatherObservation Record(
            int tenantId,
            Facility facility,
            DateTime observedAt,
            decimal temperatureCelsius,
            decimal humidityPercent,
            decimal? windSpeedKph = null,
            int? windDirectionDegrees = null,
            decimal? precipitationMm = null,
            decimal? pressureHpa = null,
            decimal? uvIndex = null,
            decimal? lightningProbabilityPercent = null,
            string providerRef = null)
        {
            if (facility == null)
            {
                throw new DomainRuleException("Facility is required for a weather observation.");
            }

            if (humidityPercent < 0 || humidityPercent > 100)
            {
                throw new DomainRuleException("Humidity must be between 0 and 100 percent.");
            }

            if (lightningProbabilityPercent.HasValue && (lightningProbabilityPercent < 0 || lightningProbabilityPercent > 100))
            {
                throw new DomainRuleException("Lightning probability must be between 0 and 100 percent.");
            }

            return new WeatherObservation
            {
                TenantId = tenantId,
                Facility = facility,
                FacilityId = facility.Id,
                ObservedAt = observedAt,
                TemperatureCelsius = temperatureCelsius,
                HumidityPercent = humidityPercent,
                WindSpeedKph = windSpeedKph,
                WindDirectionDegrees = windDirectionDegrees,
                PrecipitationMm = precipitationMm,
                PressureHpa = pressureHpa,
                UvIndex = uvIndex,
                LightningProbabilityPercent = lightningProbabilityPercent,
                ProviderRef = string.IsNullOrWhiteSpace(providerRef) ? null : providerRef.Trim()
            };
        }
    }
}
