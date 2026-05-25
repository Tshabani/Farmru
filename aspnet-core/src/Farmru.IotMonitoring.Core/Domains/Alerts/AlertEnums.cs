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
        SensorFailure = 7
    }
}
