using Farmru.IotMonitoring.Domains;
using System;

namespace Farmru.IotMonitoring.Domains.Nodes
{
    public partial class Node
    {
        public const int DefaultOfflineThresholdMinutes = 24 * 60;

        public virtual void Activate()
        {
            if (DeviceStatus == DeviceOperationalStatus.Decommissioned)
            {
                throw new DomainRuleException("Cannot activate a decommissioned device.");
            }

            IsActive = true;
            DeviceStatus = DeviceOperationalStatus.Active;
            EvaluateHealth();
        }

        public virtual void Deactivate()
        {
            if (DeviceStatus == DeviceOperationalStatus.Decommissioned)
            {
                throw new DomainRuleException("Cannot deactivate a decommissioned device.");
            }

            IsActive = false;
            DeviceStatus = DeviceOperationalStatus.Inactive;
            EvaluateHealth();
        }

        public virtual void SetMaintenance()
        {
            if (DeviceStatus == DeviceOperationalStatus.Decommissioned)
            {
                throw new DomainRuleException("Cannot place a decommissioned device in maintenance.");
            }

            DeviceStatus = DeviceOperationalStatus.Maintenance;
            EvaluateHealth();
        }

        public virtual void Decommission()
        {
            IsActive = false;
            DeviceStatus = DeviceOperationalStatus.Decommissioned;
            HealthStatus = DeviceHealthStatus.Unknown;
        }

        public virtual void RecordTelemetry(DateTime seenAt, decimal? batteryLevel, int? signalStrength, string firmwareVersion = null)
        {
            if (DeviceStatus == DeviceOperationalStatus.Decommissioned)
            {
                return;
            }

            LastSeenAt = seenAt;
            if (batteryLevel.HasValue)
            {
                BatteryLevel = Math.Clamp(batteryLevel.Value, 0, 100);
            }

            if (signalStrength.HasValue)
            {
                SignalStrength = signalStrength.Value;
            }

            if (!string.IsNullOrWhiteSpace(firmwareVersion))
            {
                FirmwareVersion = firmwareVersion.Trim();
            }

            EvaluateHealth();
        }

        public virtual bool IsOnline(int offlineThresholdMinutes = DefaultOfflineThresholdMinutes)
        {
            if (!IsActive || DeviceStatus == DeviceOperationalStatus.Decommissioned)
            {
                return false;
            }

            if (!LastSeenAt.HasValue)
            {
                return false;
            }

            return LastSeenAt.Value >= DateTime.UtcNow.AddMinutes(-offlineThresholdMinutes);
        }

        public virtual void EvaluateHealth(int offlineThresholdMinutes = DefaultOfflineThresholdMinutes)
        {
            if (DeviceStatus == DeviceOperationalStatus.Decommissioned)
            {
                HealthStatus = DeviceHealthStatus.Unknown;
                return;
            }

            if (!LastSeenAt.HasValue)
            {
                HealthStatus = DeviceHealthStatus.Unknown;
                return;
            }

            if (!IsOnline(offlineThresholdMinutes))
            {
                HealthStatus = DeviceHealthStatus.Critical;
                return;
            }

            if (TelemetryQuality == TelemetryQualityStatus.Stale || TelemetryQuality == TelemetryQualityStatus.Missing)
            {
                HealthStatus = DeviceHealthStatus.Warning;
                return;
            }

            if (TelemetryQuality == TelemetryQualityStatus.Anomaly)
            {
                HealthStatus = DeviceHealthStatus.Warning;
                return;
            }

            if (BatteryLevel.HasValue && BatteryLevel < 15)
            {
                HealthStatus = DeviceHealthStatus.Critical;
                return;
            }

            if (BatteryLevel.HasValue && BatteryLevel < 30)
            {
                HealthStatus = DeviceHealthStatus.Warning;
                return;
            }

            if (SignalStrength.HasValue && SignalStrength < 20)
            {
                HealthStatus = DeviceHealthStatus.Warning;
                return;
            }

            HealthStatus = DeviceHealthStatus.Healthy;
        }

        public static decimal? BatteryLevelFromVoltage(long? batteryVoltageMillivolts)
        {
            if (!batteryVoltageMillivolts.HasValue)
            {
                return null;
            }

            const decimal minMv = 3000m;
            const decimal maxMv = 4200m;
            var mv = (decimal)batteryVoltageMillivolts.Value;
            if (mv <= minMv)
            {
                return 0;
            }

            if (mv >= maxMv)
            {
                return 100;
            }

            return Math.Round((mv - minMv) * 100m / (maxMv - minMv), 1);
        }
    }
}
