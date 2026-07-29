using Abp.Domain.Repositories;
using Abp.Domain.Services;
using Farmru.IotMonitoring.Alerts;
using Farmru.IotMonitoring.Domains.Alerts;
using Farmru.IotMonitoring.Domains.Facilities;
using Farmru.IotMonitoring.Domains.Weather;
using Farmru.IotMonitoring.Domains.Weather.Services;
using Farmru.IotMonitoring.Services.Alerts;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Farmru.IotMonitoring.Services.Weather
{
    /// <summary>
    /// Application-layer implementation of IWeatherAlertEvaluationService. Reuses the
    /// existing Alert aggregate and IAlertRealtimeNotifier (AlertNotificationHub) rather
    /// than introducing a parallel notification path, per Phase 1 Technical Design 5.3.
    /// </summary>
    public class WeatherAlertEvaluationService : DomainService, IWeatherAlertEvaluationService
    {
        private readonly IRepository<WeatherAlertRule, Guid> _ruleRepository;
        private readonly IRepository<WeatherObservation, Guid> _observationRepository;
        private readonly IRepository<WeatherForecastDaily, Guid> _forecastRepository;
        private readonly IRepository<Facility, Guid> _facilityRepository;
        private readonly IRepository<Alert, Guid> _alertRepository;
        private readonly IAlertRealtimeNotifier _realtimeNotifier;

        public WeatherAlertEvaluationService(
            IRepository<WeatherAlertRule, Guid> ruleRepository,
            IRepository<WeatherObservation, Guid> observationRepository,
            IRepository<WeatherForecastDaily, Guid> forecastRepository,
            IRepository<Facility, Guid> facilityRepository,
            IRepository<Alert, Guid> alertRepository,
            IAlertRealtimeNotifier realtimeNotifier)
        {
            _ruleRepository = ruleRepository;
            _observationRepository = observationRepository;
            _forecastRepository = forecastRepository;
            _facilityRepository = facilityRepository;
            _alertRepository = alertRepository;
            _realtimeNotifier = realtimeNotifier;
        }

        public async Task<WeatherAlertEvaluationResult> EvaluateForTenantAsync(int tenantId)
        {
            var result = new WeatherAlertEvaluationResult();

            var rules = await _ruleRepository.GetAll()
                .Include(r => r.Facility)
                .Include(r => r.Organisation)
                .Where(r => r.TenantId == tenantId && r.IsActive)
                .ToListAsync();

            foreach (var rule in rules)
            {
                var facilities = await ResolveFacilitiesAsync(rule);
                foreach (var facility in facilities)
                {
                    result.RulesEvaluated++;
                    if (await EvaluateRuleForFacilityAsync(rule, facility))
                    {
                        result.AlertsGenerated++;
                    }
                }
            }

            return result;
        }

        private async Task<List<Facility>> ResolveFacilitiesAsync(WeatherAlertRule rule)
        {
            if (rule.FacilityId.HasValue)
            {
                var facility = await _facilityRepository.FirstOrDefaultAsync(rule.FacilityId.Value);
                return facility != null ? new List<Facility> { facility } : new List<Facility>();
            }

            return await _facilityRepository.GetAll()
                .Include(f => f.OwnerOrganisation)
                .Where(f => f.OwnerOrganisation != null && f.OwnerOrganisation.Id == rule.OrganisationId)
                .ToListAsync();
        }

        private async Task<bool> EvaluateRuleForFacilityAsync(WeatherAlertRule rule, Facility facility)
        {
            var breach = rule.AlertType switch
            {
                WeatherAlertType.Frost => await EvaluateFrostAsync(facility.Id, rule.ThresholdValue),
                WeatherAlertType.Heat => await EvaluateHeatAsync(facility.Id, rule.ThresholdValue),
                WeatherAlertType.Wind => await EvaluateWindAsync(facility.Id, rule.ThresholdValue),
                WeatherAlertType.RainSevere => await EvaluateRainAsync(facility.Id, rule.ThresholdValue),
                WeatherAlertType.Lightning => await EvaluateLightningAsync(facility.Id, rule.ThresholdValue),
                _ => (bool?)null
            };

            if (breach != true)
            {
                return false;
            }

            var alertType = MapToAlertType(rule.AlertType);
            await RaiseOrUpdateAlertAsync(rule.TenantId, facility, alertType, rule.Severity);
            return true;
        }

        private async Task<bool?> EvaluateFrostAsync(Guid facilityId, decimal thresholdCelsius)
        {
            var forecast = await LatestForecastAsync(facilityId);
            return forecast != null && forecast.TempMinCelsius <= thresholdCelsius;
        }

        private async Task<bool?> EvaluateHeatAsync(Guid facilityId, decimal thresholdCelsius)
        {
            var forecast = await LatestForecastAsync(facilityId);
            return forecast != null && forecast.TempMaxCelsius >= thresholdCelsius;
        }

        private async Task<bool?> EvaluateWindAsync(Guid facilityId, decimal thresholdKph)
        {
            var observation = await LatestObservationAsync(facilityId);
            var forecast = await LatestForecastAsync(facilityId);
            var observedBreach = observation?.WindSpeedKph.HasValue == true && observation.WindSpeedKph.Value >= thresholdKph;
            var forecastBreach = forecast?.WindGustKph.HasValue == true && forecast.WindGustKph.Value >= thresholdKph;
            return observedBreach || forecastBreach;
        }

        private async Task<bool?> EvaluateRainAsync(Guid facilityId, decimal thresholdMm)
        {
            var observation = await LatestObservationAsync(facilityId);
            return observation?.PrecipitationMm.HasValue == true && observation.PrecipitationMm.Value >= thresholdMm;
        }

        private async Task<bool?> EvaluateLightningAsync(Guid facilityId, decimal thresholdProbabilityPercent)
        {
            var observation = await LatestObservationAsync(facilityId);
            return observation?.LightningProbabilityPercent.HasValue == true
                && observation.LightningProbabilityPercent.Value >= thresholdProbabilityPercent;
        }

        private async Task<WeatherObservation> LatestObservationAsync(Guid facilityId)
        {
            return await _observationRepository.GetAll()
                .Where(o => o.FacilityId == facilityId)
                .OrderByDescending(o => o.ObservedAt)
                .FirstOrDefaultAsync();
        }

        private async Task<WeatherForecastDaily> LatestForecastAsync(Guid facilityId)
        {
            return await _forecastRepository.GetAll()
                .Where(f => f.FacilityId == facilityId && f.ForecastFor == DateTime.UtcNow.Date)
                .OrderByDescending(f => f.GeneratedAt)
                .FirstOrDefaultAsync();
        }

        private static AlertType MapToAlertType(WeatherAlertType weatherAlertType) => weatherAlertType switch
        {
            WeatherAlertType.Frost => AlertType.WeatherFrost,
            WeatherAlertType.Heat => AlertType.WeatherHeatStress,
            WeatherAlertType.Wind => AlertType.WeatherHighWind,
            WeatherAlertType.Lightning => AlertType.WeatherLightning,
            WeatherAlertType.RainSevere => AlertType.WeatherSevereRain,
            _ => throw new ArgumentOutOfRangeException(nameof(weatherAlertType), weatherAlertType, "Unknown weather alert type.")
        };

        private async Task RaiseOrUpdateAlertAsync(int tenantId, Facility facility, AlertType alertType, AlertSeverity severity)
        {
            var existing = await _alertRepository.GetAll()
                .FirstOrDefaultAsync(a =>
                    a.TenantId == tenantId &&
                    a.FacilityId == facility.Id &&
                    a.AlertType == alertType &&
                    a.IsActive &&
                    !a.IsResolved);

            if (existing != null)
            {
                existing.UpdateLastTriggered(severity);
                await _alertRepository.UpdateAsync(existing);
                await NotifyAsync(existing, "updated");
                return;
            }

            var title = $"Weather alert: {alertType}";
            var description = $"Weather condition for {alertType} breached the configured threshold at {facility.Name}.";
            var alert = Alert.Raise(tenantId, null, facility.Id, alertType, severity, title, description);
            await _alertRepository.InsertAsync(alert);
            await NotifyAsync(alert, "created");
        }

        private async Task NotifyAsync(Alert alert, string action)
        {
            if (_realtimeNotifier == null)
            {
                return;
            }

            await _realtimeNotifier.NotifyAsync(AlertMappingHelper.ToDto(alert), action);
        }
    }
}
