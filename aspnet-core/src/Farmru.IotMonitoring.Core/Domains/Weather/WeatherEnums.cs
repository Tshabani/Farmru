namespace Farmru.IotMonitoring.Domains.Weather
{
    public enum FrostRiskLevel
    {
        None = 0,
        Watch = 1,
        Warning = 2
    }

    public enum HeatStressLevel
    {
        None = 0,
        Elevated = 1,
        Severe = 2
    }

    /// <summary>
    /// The weather condition a WeatherAlertRule watches. Distinct from the existing
    /// Alerts.AlertType enum (see ADR-009): this enum drives which threshold a rule
    /// evaluates; the resulting Alert is then raised with the matching AlertType
    /// (e.g. WeatherAlertType.Frost -> AlertType.WeatherFrost).
    /// </summary>
    public enum WeatherAlertType
    {
        Frost = 0,
        Wind = 1,
        Heat = 2,
        Lightning = 3,
        RainSevere = 4
    }
}
