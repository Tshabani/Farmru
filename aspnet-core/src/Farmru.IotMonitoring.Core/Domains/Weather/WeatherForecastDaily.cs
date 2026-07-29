using Abp.Domain.Entities;
using Abp.Domain.Entities.Auditing;
using Farmru.IotMonitoring.Domains;
using Farmru.IotMonitoring.Domains.Facilities;
using System;

namespace Farmru.IotMonitoring.Domains.Weather
{
    /// <summary>
    /// A daily forecast row for a Facility. Append-only by design (Phase 1 Technical Design
    /// Section 2.1): a new row is written per fetch rather than updating an existing forecast
    /// in place, so forecast-accuracy history is preserved for free.
    /// </summary>
    public class WeatherForecastDaily : FullAuditedAggregateRoot<Guid>, IMustHaveTenant
    {
        protected WeatherForecastDaily()
        {
        }

        public int TenantId { get; set; }
        public virtual Guid FacilityId { get; private set; }
        public virtual Facility Facility { get; private set; }
        public virtual DateTime ForecastFor { get; private set; }
        public virtual DateTime GeneratedAt { get; private set; }
        public virtual decimal TempMinCelsius { get; private set; }
        public virtual decimal TempMaxCelsius { get; private set; }
        public virtual int PrecipitationProbabilityPercent { get; private set; }
        public virtual decimal? WindGustKph { get; private set; }
        public virtual FrostRiskLevel FrostRisk { get; private set; }
        public virtual HeatStressLevel HeatStress { get; private set; }
        public virtual string ProviderRef { get; private set; }

        public static WeatherForecastDaily Record(
            int tenantId,
            Facility facility,
            DateTime forecastFor,
            DateTime generatedAt,
            decimal tempMinCelsius,
            decimal tempMaxCelsius,
            int precipitationProbabilityPercent,
            decimal? windGustKph = null,
            FrostRiskLevel frostRisk = FrostRiskLevel.None,
            HeatStressLevel heatStress = HeatStressLevel.None,
            string providerRef = null)
        {
            if (facility == null)
            {
                throw new DomainRuleException("Facility is required for a weather forecast.");
            }

            if (forecastFor.Date < generatedAt.Date)
            {
                throw new DomainRuleException("A weather forecast cannot be recorded for a past date.");
            }

            if (precipitationProbabilityPercent < 0 || precipitationProbabilityPercent > 100)
            {
                throw new DomainRuleException("Precipitation probability must be between 0 and 100 percent.");
            }

            if (tempMaxCelsius < tempMinCelsius)
            {
                throw new DomainRuleException("Forecast maximum temperature cannot be lower than the minimum.");
            }

            return new WeatherForecastDaily
            {
                TenantId = tenantId,
                Facility = facility,
                FacilityId = facility.Id,
                ForecastFor = forecastFor.Date,
                GeneratedAt = generatedAt,
                TempMinCelsius = tempMinCelsius,
                TempMaxCelsius = tempMaxCelsius,
                PrecipitationProbabilityPercent = precipitationProbabilityPercent,
                WindGustKph = windGustKph,
                FrostRisk = frostRisk,
                HeatStress = heatStress,
                ProviderRef = string.IsNullOrWhiteSpace(providerRef) ? null : providerRef.Trim()
            };
        }
    }
}
