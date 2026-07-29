namespace Farmru.IotMonitoring.Domains.Alerts
{
    public enum AlertSeverity
    {
        Info = 0,
        Warning = 1,
        Critical = 2
    }

    public enum AlertType
    {
        DeviceOffline = 0,
        LowBattery = 1,
        SoilMoistureLow = 2,
        TemperatureHigh = 3,
        TemperatureLow = 4,
        TelemetryAnomaly = 5,
        GeoFenceBreach = 6,
        SensorFailure = 7,

        // Weather Intelligence (Phase 1) — see ADR-009: Alert has no separate AlertSource
        // concept, so weather-driven alerts are represented as AlertType values rather than
        // a new classification field.
        WeatherFrost = 8,
        WeatherHeatStress = 9,
        WeatherHighWind = 10,
        WeatherLightning = 11,
        WeatherSevereRain = 12
    }
}
