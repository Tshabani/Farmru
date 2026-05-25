namespace Farmru.IotMonitoring.Domains.Nodes
{
    public enum DeviceOperationalStatus
    {
        Active = 0,
        Inactive = 1,
        Maintenance = 2,
        Decommissioned = 3
    }

    public enum DeviceHealthStatus
    {
        Unknown = 0,
        Healthy = 1,
        Warning = 2,
        Critical = 3
    }

    public enum TelemetryQualityStatus
    {
        Unknown = 0,
        Good = 1,
        Stale = 2,
        Missing = 3,
        Anomaly = 4
    }
}
