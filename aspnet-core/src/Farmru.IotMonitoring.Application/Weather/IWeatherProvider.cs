using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace Farmru.IotMonitoring.Weather
{
    /// <summary>
    /// External weather data source abstraction (Phase 1 Technical Design Section 5.1).
    /// No concrete implementation is registered yet: vendor selection is ADR-001, an open
    /// business/commercial decision — see /docs/implementation/phase1/Sprint0-004-WeatherProvider.md.
    /// WeatherSyncHostedService is intentionally NOT registered in Startup.cs until a
    /// concrete provider is bound, to avoid a DI resolution failure at application startup.
    /// </summary>
    public interface IWeatherProvider
    {
        Task<WeatherProviderCurrentResult> GetCurrentAsync(decimal latitude, decimal longitude);
        Task<List<WeatherProviderForecastDayResult>> GetForecastAsync(decimal latitude, decimal longitude, int days = 7);
        Task<List<WeatherProviderHistoricalResult>> GetHistoryAsync(decimal latitude, decimal longitude, DateTime from, DateTime to);
    }

    public class WeatherProviderCurrentResult
    {
        public DateTime ObservedAt { get; set; }
        public decimal TemperatureCelsius { get; set; }
        public decimal HumidityPercent { get; set; }
        public decimal? WindSpeedKph { get; set; }
        public int? WindDirectionDegrees { get; set; }
        public decimal? PrecipitationMm { get; set; }
        public decimal? PressureHpa { get; set; }
        public decimal? UvIndex { get; set; }
        public decimal? LightningProbabilityPercent { get; set; }
        public string ProviderRef { get; set; }
    }

    public class WeatherProviderForecastDayResult
    {
        public DateTime ForecastFor { get; set; }
        public decimal TempMinCelsius { get; set; }
        public decimal TempMaxCelsius { get; set; }
        public int PrecipitationProbabilityPercent { get; set; }
        public decimal? WindGustKph { get; set; }
        public string ProviderRef { get; set; }
    }

    public class WeatherProviderHistoricalResult
    {
        public DateTime ObservedAt { get; set; }
        public decimal TemperatureCelsius { get; set; }
        public decimal HumidityPercent { get; set; }
        public decimal? PrecipitationMm { get; set; }
        public string ProviderRef { get; set; }
    }
}
