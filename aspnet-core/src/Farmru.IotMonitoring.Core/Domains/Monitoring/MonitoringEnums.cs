namespace Farmru.IotMonitoring.Domains.Monitoring
{
    public enum MonitoringJobType
    {
        DeviceOffline = 0,
        TelemetryHealth = 1,
        AlertEscalation = 2,
        FullCycle = 3,
        IncidentSla = 4
    }
}
