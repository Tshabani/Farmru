using Abp.Application.Services.Dto;
using Farmru.IotMonitoring.Helpers;
using System;

namespace Farmru.IotMonitoring.Services.Alerts.Dto
{
    public class AlertThresholdConfigurationDto : EntityDto<Guid>
    {
        public EntityWithDisplayNameDto<Guid?> Facility { get; set; }
        public decimal MinimumBatteryPercent { get; set; }
        public decimal CriticalBatteryPercent { get; set; }
        public decimal MaximumTemperature { get; set; }
        public decimal MinimumTemperature { get; set; }
        public decimal MinimumMoisturePercent { get; set; }
        public int OfflineTimeoutMinutes { get; set; }
        public bool AutoResolveWhenNormalized { get; set; }
        public int StaleTelemetryThresholdMinutes { get; set; }
        public int EscalationTimeoutMinutes { get; set; }
        public int AnomalySensitivityPercent { get; set; }
        public bool MonitoringEnabled { get; set; }
    }

    public class UpdateAlertThresholdConfigurationInput
    {
        public Guid? FacilityId { get; set; }
        public decimal MinimumBatteryPercent { get; set; }
        public decimal CriticalBatteryPercent { get; set; }
        public decimal MaximumTemperature { get; set; }
        public decimal MinimumTemperature { get; set; }
        public decimal MinimumMoisturePercent { get; set; }
        public int OfflineTimeoutMinutes { get; set; }
        public bool AutoResolveWhenNormalized { get; set; }
        public int StaleTelemetryThresholdMinutes { get; set; }
        public int EscalationTimeoutMinutes { get; set; }
        public int AnomalySensitivityPercent { get; set; }
        public bool MonitoringEnabled { get; set; }
    }
}
