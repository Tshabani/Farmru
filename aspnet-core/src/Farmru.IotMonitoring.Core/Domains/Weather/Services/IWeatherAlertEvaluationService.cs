using Abp.Domain.Services;
using System.Threading.Tasks;

namespace Farmru.IotMonitoring.Domains.Weather.Services
{
    /// <summary>
    /// Evaluates active WeatherAlertRules against the latest WeatherObservation/
    /// WeatherForecastDaily for their scope (Facility or Organisation), raising the
    /// existing Alert aggregate on breach (Phase 1 Technical Design Section 5.1/5.2,
    /// ADR-009 — alerts are classified via AlertType, not a separate AlertSource).
    /// </summary>
    public interface IWeatherAlertEvaluationService : IDomainService
    {
        Task<WeatherAlertEvaluationResult> EvaluateForTenantAsync(int tenantId);
    }

    public class WeatherAlertEvaluationResult
    {
        public int RulesEvaluated { get; set; }
        public int AlertsGenerated { get; set; }
    }
}
