using Abp.Application.Services.Dto;
using Abp.Authorization;
using Abp.Domain.Repositories;
using Abp.Linq.Extensions;
using Abp.UI;
using Farmru.IotMonitoring.Authorization;
using Farmru.IotMonitoring.Domains.Weather;
using Farmru.IotMonitoring.Services.Weather.Dto;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Farmru.IotMonitoring.Services.Weather
{
    [AbpAuthorize(PermissionNames.Pages_Weather)]
    public class WeatherAppService : IotMonitoringAppServiceBase, IWeatherAppService
    {
        private readonly IRepository<WeatherObservation, Guid> _observationRepository;
        private readonly IRepository<WeatherForecastDaily, Guid> _forecastRepository;
        private readonly IRepository<EvapotranspirationReading, Guid> _etRepository;

        public WeatherAppService(
            IRepository<WeatherObservation, Guid> observationRepository,
            IRepository<WeatherForecastDaily, Guid> forecastRepository,
            IRepository<EvapotranspirationReading, Guid> etRepository)
        {
            _observationRepository = observationRepository;
            _forecastRepository = forecastRepository;
            _etRepository = etRepository;
        }

        public async Task<WeatherObservationDto> GetCurrent(EntityDto<Guid> facilityId)
        {
            var latest = await _observationRepository.GetAll()
                .Where(o => o.FacilityId == facilityId.Id)
                .OrderByDescending(o => o.ObservedAt)
                .FirstOrDefaultAsync();

            if (latest == null)
            {
                throw new UserFriendlyException(L("NoWeatherDataAvailable"));
            }

            return MapToDto(latest);
        }

        public async Task<List<WeatherForecastDto>> GetForecast(EntityDto<Guid> facilityId)
        {
            // Latest GeneratedAt per ForecastFor date, per Technical Design Section 4.1 —
            // WeatherForecastDaily is append-only (Section 2.1), so multiple rows can exist
            // per date; only the most recently fetched forecast for each date is returned.
            var candidates = await _forecastRepository.GetAll()
                .Where(f => f.FacilityId == facilityId.Id && f.ForecastFor >= DateTime.UtcNow.Date)
                .ToListAsync();

            var latestPerDay = candidates
                .GroupBy(f => f.ForecastFor)
                .Select(g => g.OrderByDescending(f => f.GeneratedAt).First())
                .OrderBy(f => f.ForecastFor)
                .Take(7)
                .ToList();

            return latestPerDay.Select(MapToDto).ToList();
        }

        public async Task<PagedResultDto<WeatherObservationDto>> GetHistory(GetWeatherHistoryInput input)
        {
            var query = _observationRepository.GetAll()
                .Where(o => o.FacilityId == input.FacilityId);

            if (input.FromDate.HasValue)
            {
                query = query.Where(o => o.ObservedAt >= input.FromDate.Value);
            }

            if (input.ToDate.HasValue)
            {
                query = query.Where(o => o.ObservedAt <= input.ToDate.Value);
            }

            var totalCount = await query.CountAsync();
            var items = await query
                .OrderByDescending(o => o.ObservedAt)
                .PageBy(input)
                .ToListAsync();

            return new PagedResultDto<WeatherObservationDto>(totalCount, items.Select(MapToDto).ToList());
        }

        public async Task<List<EvapotranspirationDto>> GetEvapotranspiration(GetEvapotranspirationInput input)
        {
            var query = _etRepository.GetAll().Where(e => e.FacilityId == input.FacilityId);

            if (input.FromDate.HasValue)
            {
                query = query.Where(e => e.Date >= input.FromDate.Value);
            }

            if (input.ToDate.HasValue)
            {
                query = query.Where(e => e.Date <= input.ToDate.Value);
            }

            var items = await query.OrderByDescending(e => e.Date).Take(90).ToListAsync();

            return items.Select(e => new EvapotranspirationDto
            {
                Id = e.Id,
                FacilityId = e.FacilityId,
                Date = e.Date,
                Et0Mm = e.Et0Mm,
                EtcMm = e.EtcMm,
                CropSeasonId = e.CropSeasonId
            }).ToList();
        }

        private static WeatherObservationDto MapToDto(WeatherObservation observation) => new WeatherObservationDto
        {
            Id = observation.Id,
            FacilityId = observation.FacilityId,
            ObservedAt = observation.ObservedAt,
            TemperatureCelsius = observation.TemperatureCelsius,
            HumidityPercent = observation.HumidityPercent,
            WindSpeedKph = observation.WindSpeedKph,
            WindDirectionDegrees = observation.WindDirectionDegrees,
            PrecipitationMm = observation.PrecipitationMm,
            PressureHpa = observation.PressureHpa,
            UvIndex = observation.UvIndex,
            LightningProbabilityPercent = observation.LightningProbabilityPercent
        };

        private static WeatherForecastDto MapToDto(WeatherForecastDaily forecast) => new WeatherForecastDto
        {
            Id = forecast.Id,
            FacilityId = forecast.FacilityId,
            ForecastFor = forecast.ForecastFor,
            GeneratedAt = forecast.GeneratedAt,
            TempMinCelsius = forecast.TempMinCelsius,
            TempMaxCelsius = forecast.TempMaxCelsius,
            PrecipitationProbabilityPercent = forecast.PrecipitationProbabilityPercent,
            WindGustKph = forecast.WindGustKph,
            FrostRisk = forecast.FrostRisk,
            HeatStress = forecast.HeatStress
        };
    }
}
