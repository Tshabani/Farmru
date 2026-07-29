using Abp.Dependency;
using System.Threading.Tasks;

namespace Farmru.IotMonitoring.Services.Weather
{
    /// <summary>
    /// Fetches current conditions and forecast from IWeatherProvider for every active
    /// Facility and persists WeatherObservation/WeatherForecastDaily rows (Phase 1
    /// Technical Design Section 5.1/5.2). Driven by WeatherSyncHostedService, which is
    /// not yet registered in Startup.cs pending ADR-001 (weather provider selection).
    /// </summary>
    public interface IWeatherSyncEngine : ITransientDependency
    {
        Task RunFullSyncCycleAsync();
    }
}
