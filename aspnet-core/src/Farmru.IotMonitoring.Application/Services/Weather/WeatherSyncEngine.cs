using Abp.Domain.Repositories;
using Abp.Domain.Uow;
using Farmru.IotMonitoring.Domains.Facilities;
using Farmru.IotMonitoring.Domains.Weather;
using Farmru.IotMonitoring.MultiTenancy;
using Farmru.IotMonitoring.Weather;
using Microsoft.EntityFrameworkCore;
using System;
using System.Linq;
using System.Threading.Tasks;

namespace Farmru.IotMonitoring.Services.Weather
{
    /// <summary>
    /// Per-tenant, per-Facility weather sync — mirrors OperationalMonitoringEngine's
    /// tenant-iteration shape (Phase 1 Technical Design Section 5.2 / Sprint0-001).
    /// </summary>
    public class WeatherSyncEngine : IWeatherSyncEngine
    {
        private readonly IRepository<Tenant> _tenantRepository;
        private readonly IRepository<Facility, Guid> _facilityRepository;
        private readonly IRepository<WeatherObservation, Guid> _observationRepository;
        private readonly IRepository<WeatherForecastDaily, Guid> _forecastRepository;
        private readonly IWeatherProvider _weatherProvider;
        private readonly IUnitOfWorkManager _unitOfWorkManager;

        public WeatherSyncEngine(
            IRepository<Tenant> tenantRepository,
            IRepository<Facility, Guid> facilityRepository,
            IRepository<WeatherObservation, Guid> observationRepository,
            IRepository<WeatherForecastDaily, Guid> forecastRepository,
            IWeatherProvider weatherProvider,
            IUnitOfWorkManager unitOfWorkManager)
        {
            _tenantRepository = tenantRepository;
            _facilityRepository = facilityRepository;
            _observationRepository = observationRepository;
            _forecastRepository = forecastRepository;
            _weatherProvider = weatherProvider;
            _unitOfWorkManager = unitOfWorkManager;
        }

        public async Task RunFullSyncCycleAsync()
        {
            var tenantIds = await _tenantRepository.GetAll()
                .Where(t => t.IsActive)
                .Select(t => t.Id)
                .ToListAsync();

            foreach (var tenantId in tenantIds)
            {
                using var uow = _unitOfWorkManager.Begin();
                using (_unitOfWorkManager.Current.SetTenantId(tenantId))
                {
                    await SyncTenantAsync(tenantId);
                    await uow.CompleteAsync();
                }
            }
        }

        private async Task SyncTenantAsync(int tenantId)
        {
            // Facility does not implement IMustHaveTenant itself — tenancy flows through
            // OwnerOrganisation, matching how the rest of the domain (e.g. NodeDataAppService.GetReadings)
            // already resolves a tenant's Facilities.
            var facilities = await _facilityRepository.GetAll()
                .Include(f => f.OwnerOrganisation)
                .Where(f => f.OwnerOrganisation != null && f.OwnerOrganisation.TenantId == tenantId
                    && f.Latitude.HasValue && f.Longitude.HasValue)
                .ToListAsync();

            foreach (var facility in facilities)
            {
                await SyncFacilityAsync(tenantId, facility);
            }
        }

        private async Task SyncFacilityAsync(int tenantId, Facility facility)
        {
            var current = await _weatherProvider.GetCurrentAsync(facility.Latitude.Value, facility.Longitude.Value);
            var observation = WeatherObservation.Record(
                tenantId,
                facility,
                current.ObservedAt,
                current.TemperatureCelsius,
                current.HumidityPercent,
                current.WindSpeedKph,
                current.WindDirectionDegrees,
                current.PrecipitationMm,
                current.PressureHpa,
                current.UvIndex,
                current.LightningProbabilityPercent,
                current.ProviderRef);
            await _observationRepository.InsertAsync(observation);

            var forecastDays = await _weatherProvider.GetForecastAsync(facility.Latitude.Value, facility.Longitude.Value);
            var generatedAt = DateTime.UtcNow;
            foreach (var day in forecastDays)
            {
                var forecast = WeatherForecastDaily.Record(
                    tenantId,
                    facility,
                    day.ForecastFor,
                    generatedAt,
                    day.TempMinCelsius,
                    day.TempMaxCelsius,
                    day.PrecipitationProbabilityPercent,
                    day.WindGustKph,
                    providerRef: day.ProviderRef);
                await _forecastRepository.InsertAsync(forecast);
            }
        }
    }
}
