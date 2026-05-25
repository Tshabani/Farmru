using Abp.Domain.Entities;
using Abp.Domain.Entities.Auditing;
using Farmru.IotMonitoring.Domains;
using Farmru.IotMonitoring.Domains.Facilities;
using System;

namespace Farmru.IotMonitoring.Domains.Alerts
{
    public class AlertThresholdConfiguration : FullAuditedEntity<Guid>, IMustHaveTenant
    {
        protected AlertThresholdConfiguration()
        {
        }

        public int TenantId { get; set; }
        public virtual Guid? FacilityId { get; private set; }
        public virtual Facility Facility { get; private set; }
        public virtual decimal MinimumBatteryPercent { get; private set; } = 20;
        public virtual decimal CriticalBatteryPercent { get; private set; } = 10;
        public virtual decimal MaximumTemperature { get; private set; } = 35;
        public virtual decimal MinimumTemperature { get; private set; } = 5;
        public virtual decimal MinimumMoisturePercent { get; private set; } = 25;
        public virtual int OfflineTimeoutMinutes { get; private set; } = 24 * 60;
        public virtual bool AutoResolveWhenNormalized { get; private set; } = true;
        public virtual int StaleTelemetryThresholdMinutes { get; private set; } = 120;
        public virtual int EscalationTimeoutMinutes { get; private set; } = 60;
        public virtual int AnomalySensitivityPercent { get; private set; } = 50;
        public virtual bool MonitoringEnabled { get; private set; } = true;

        public static AlertThresholdConfiguration CreateDefault(int tenantId, Facility facility = null)
        {
            return new AlertThresholdConfiguration
            {
                TenantId = tenantId,
                Facility = facility,
                FacilityId = facility?.Id
            };
        }

        public virtual void Update(
            decimal minimumBatteryPercent,
            decimal criticalBatteryPercent,
            decimal maximumTemperature,
            decimal minimumTemperature,
            decimal minimumMoisturePercent,
            int offlineTimeoutMinutes,
            bool autoResolveWhenNormalized,
            int staleTelemetryThresholdMinutes,
            int escalationTimeoutMinutes,
            int anomalySensitivityPercent,
            bool monitoringEnabled)
        {
            if (minimumBatteryPercent < 0 || minimumBatteryPercent > 100)
            {
                throw new DomainRuleException("Minimum battery must be between 0 and 100.");
            }

            if (criticalBatteryPercent < 0 || criticalBatteryPercent > minimumBatteryPercent)
            {
                throw new DomainRuleException("Critical battery must be between 0 and minimum battery threshold.");
            }

            MinimumBatteryPercent = minimumBatteryPercent;
            CriticalBatteryPercent = criticalBatteryPercent;
            MaximumTemperature = maximumTemperature;
            MinimumTemperature = minimumTemperature;
            MinimumMoisturePercent = minimumMoisturePercent;
            OfflineTimeoutMinutes = offlineTimeoutMinutes < 5 ? 5 : offlineTimeoutMinutes;
            AutoResolveWhenNormalized = autoResolveWhenNormalized;
            StaleTelemetryThresholdMinutes = staleTelemetryThresholdMinutes < 15 ? 15 : staleTelemetryThresholdMinutes;
            EscalationTimeoutMinutes = escalationTimeoutMinutes < 15 ? 15 : escalationTimeoutMinutes;
            AnomalySensitivityPercent = anomalySensitivityPercent < 0 ? 0 : (anomalySensitivityPercent > 100 ? 100 : anomalySensitivityPercent);
            MonitoringEnabled = monitoringEnabled;
        }
    }
}
